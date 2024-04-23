using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        //在class內創造一個private field，將private field 指派給context(快速修復 -> Initialize field from parameter)
        //快捷ctor
        private readonly StoreContext _context;

        public ProductsController(StoreContext context)
        {
            _context = context;

        }

        //返回所有產品列表
        [HttpGet]
        //async達成異步 -> 讓多人可以同時打接口
        public async Task<ActionResult<List<Product>>>GetProducts()
        {
            return await context.Products.ToListAsync();
        }

        //傳入id參數 返回單一產品
        [HttpGet("{id}")]  //api/product/3
        public async Task<ActionResult<Product>>GetProduct(int id)
        {
            return await context.Products.FindAsync(id);
        }
    }
}