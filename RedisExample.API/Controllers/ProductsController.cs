using Microsoft.AspNetCore.Mvc;
using RedisExample.API.Models;
using RedisExample.API.Repository;
using RedisExampleApp.Cache.Service;
using StackExchange.Redis;

namespace RedisExample.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IDatabase _database;
       

        public ProductsController(IProductRepository productRepository, IDatabase database  )
        {
            _productRepository = productRepository;
            _database = database;

            _database.StringSet("soyad", "yıldız");
    
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var values = await _productRepository.GetAsync();
            return Ok(values);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductId(int id)
        {
            var values = await _productRepository.GetByIdAsync(id);
            return Ok(values);
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            await _productRepository.CreateASync(product);
            return Ok("Başarılı bir şekilde eklendi");
        }
    }
}
