using RedisExample.API.Models;
using RedisExampleApp.Cache.Service;
using StackExchange.Redis;
using System.Text.Json;
using System.Threading.RateLimiting;

namespace RedisExample.API.Repository
{
    public class ProductRepositoryWithCache : IProductRepository
    {
        private const string productKey = "productCaches"; //REdiste tutacağım cache ismimiz
        private readonly IProductRepository _repository;
        private readonly RedisService _redisService;
        private readonly IDatabase _cacheRepository;
        public ProductRepositoryWithCache(IProductRepository repository, RedisService redisService, IDatabase cacheRepository)
        {
            _repository = repository;
            _redisService = redisService;
            _cacheRepository = cacheRepository;

            _cacheRepository = _redisService.GetDb(2);
        }

        public async Task<Product> CreateASync(Product product)
        {
            var products = await _repository.CreateASync(product);

            if (await _cacheRepository.KeyExistsAsync(productKey))
            {
                await _cacheRepository.HashSetAsync(productKey, product.ProductID, JsonSerializer.Serialize(products));
            }

            return products;
        }

        public async Task<List<Product>> GetAsync()
        {

            if (!await _cacheRepository.KeyExistsAsync(productKey))//Data yoksa demek
                return await LoadCacheFromDbAsync();

            var products = new List<Product>();

            var cacheProducts = await _cacheRepository.HashGetAllAsync(productKey);

            foreach (var item in cacheProducts.ToList())
            {
                var product = JsonSerializer.Deserialize<Product>(item.Value);
                products.Add(product);
            }
            return products;


        }

        public async Task<Product> GetByIdAsync(int id)
        {
            if (_cacheRepository.KeyExists(productKey))
            {
                var product = await _cacheRepository.HashGetAsync(productKey, id);
                return product.HasValue ? JsonSerializer.Deserialize<Product>(product) : null;
            }
            var products = await LoadCacheFromDbAsync();
            return products.FirstOrDefault(x => x.ProductID == id);
        }

        private async Task<List<Product>> LoadCacheFromDbAsync()
        {
            var products = await _repository.GetAsync();
            products.ForEach(p =>
            {
                _cacheRepository.HashSetAsync(productKey, p.ProductID, JsonSerializer.Serialize(p)); //id üstünden tüm datayı alabiliiriz
            });
            return products;  //Db datadayı cacheleyecek

        }

    }
}
