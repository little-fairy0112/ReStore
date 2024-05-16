using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class BasketController : BaseApiController
    {
        private readonly StoreContext _context;
        public BasketController(StoreContext context)
        {
            _context = context;
            
        }

        // 取得購物車內的商品(命名為GetBasket)
        [HttpGet(Name = "GetBasket")]
        public async Task<ActionResult<BasketDto>> GetBasket()
        {
            var basket = await RetrieveBasket();

            if (basket == null) return NotFound();
            return MapBasketToDto(basket);
        }

        // 新增商品到購物車
        //int productId, int quantity是要傳入的query string(api/basket?productId=3&quantity=2)
        [HttpPost]
        public async Task<ActionResult<BasketDto>> AddItemToBasket(int productId, int quantity)
        {
            // 步驟1: get basket(使用buyerId獲取basket) || create basket(如果沒有basket則創建basket)
            var basket = await RetrieveBasket();
            if (basket == null) basket = CreateBasket();

            // 步驟2: get product
            var product = await _context.Products.FindAsync(productId);
            //檢查product有沒有貨
            if (product == null) return NotFound();

            // 步驟3: add item
            basket.AddItem(product, quantity);

            // 步驟4: save changes
            // SaveChangesAsync()會回傳一個值，說明資料庫修改的數量，>0代表有成功修改
            var result = await _context.SaveChangesAsync() > 0;
            //成功
            if (result) return CreatedAtRoute("GetBasket", MapBasketToDto(basket));
            //失敗
            return BadRequest(new ProblemDetails{Title = "Problem saving item to basket"});
        }

        [HttpDelete]
        public async Task<ActionResult> RemoveBasketItem(int productId, int quantity)
        {
            // 步驟1: get basket
            var basket = await RetrieveBasket();
            if (basket == null) return NotFound();

            // 步驟2: remove item or reduce quantity
            basket.RemoveItem(productId, quantity);

            // 步驟3: save changes
            var result = await _context.SaveChangesAsync() > 0;
            //成功
            if (result) return Ok();
            //失敗
            return BadRequest(new ProblemDetails{Title = "Problem removing item from the basket"});
        }

        private async Task<Basket> RetrieveBasket()
        {
            return await _context.Baskets
                // 使用cookie在client端與server端來回傳遞
                // 當用戶創建basket時 返回買家id
                .Include(i => i.Items)
                .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(x => x.BuyerId == Request.Cookies["buyerId"]);
        }

        private Basket CreateBasket()
        {
            // 隨機生成buyerId
            var buyerId = Guid.NewGuid().ToString();
            // 創建cookie, cookie是essential必要的、
            var cookieOptions = new CookieOptions{IsEssential = true, Expires = DateTime.Now.AddDays(30)};
            Response.Cookies.Append("buyerId", buyerId, cookieOptions);
            // create new basket
            var basket = new Basket{BuyerId = buyerId};
            // 追蹤剛創建的新basket
            _context.Baskets.Add(basket);
            return basket;
        }   

        private BasketDto MapBasketToDto(Basket basket)
        {
            return new BasketDto
            {
                Id = basket.Id,
                BuyerId = basket.BuyerId,
                Items = basket.Items.Select(item => new BasketItemDto
                {
                    ProductId = item.ProductId,
                    Name = item.Product.Name,
                    Price = item.Product.Price,
                    PictureUrl = item.Product.PictureUrl,
                    Type = item.Product.Type,
                    Brand = item.Product.Brand,
                    Quantity = item.Quantity
                }).ToList()
            };
        }

    }
}