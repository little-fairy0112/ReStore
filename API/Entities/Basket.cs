using System.Collections.Generic;

namespace API.Entities
{
    public class Basket
    {
        public int Id { get; set; }
        //為了避免buyer在沒有登入的情況下將商品加入購物車 所以要給buyer一個隨機id
        //這樣我們可以追蹤這個basket屬於哪個buyer
        public string BuyerId { get; set; }

        //每創建一個basket 都會創建一個新的列表
        public List<BasketItem> Items { get; set; } = new();

        public void AddItem(Product product, int quantity)
        {
            //要先判斷籃子內有沒有這個商品
            //如果籃子內已經有這個商品，則商品數量+1
            //如果籃子裡不包含這個商品，則當作一個新商品加入籃子內，數量為1

            //首先 確定產品不在籃子列表內
            if(Items.All(item => item.ProductId != product.Id))
            {
                Items.Add(new BasketItem{Product = product, Quantity = quantity});
            }

            var existingItem = Items.FirstOrDefault(item => item.ProductId == product.Id);
            if(existingItem != null) existingItem.Quantity += quantity;
        }

        public void RemoveItem(int ProductId, int quantity)
        {
            var item = Items.FirstOrDefault(item => item.ProductId == ProductId);
            //檢查物品是不是空的，如果是空的就不能減少數量，return退出這個method
            if(item == null) return;
            //如果確實有物品，則減去quantity
            item.Quantity -= quantity;
            //如果quantity已經為零，則從remove該商品
            if(item.Quantity == 0) Items.Remove(item);
        }
    }
}