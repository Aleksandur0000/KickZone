using Dapper;
using KickZone.Models;
using MySqlConnector;

namespace KickZone.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("KickZone")
                ?? throw new InvalidOperationException("Missing KickZone connection string");
        }

        private MySqlConnection Open()
        {
            var connection = new MySqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        public List<ShoesModel> GetShoes(int? categoryId = null, string? brand = null, string? search = null)
        {
            using var db = Open();
            var sql = @"
                SELECT s.id, s.name, s.description, s.price, s.image_url AS ImageUrl,
                       s.size, s.color, s.brand, s.category_id AS CategoryId, s.stock,
                       c.name AS CategoryName
                FROM shoes s
                JOIN categories c ON c.id = s.category_id
                WHERE 1 = 1";

            if (categoryId is not null)
                sql += " AND s.category_id = @categoryId";
            if (!string.IsNullOrWhiteSpace(brand))
                sql += " AND s.brand = @brand";
            if (!string.IsNullOrWhiteSpace(search))
                sql += " AND (s.name LIKE @q OR s.brand LIKE @q OR IFNULL(s.description,'') LIKE @q)";

            sql += " ORDER BY s.id";

            return db.Query<ShoesModel>(sql, new
            {
                categoryId,
                brand,
                q = "%" + (search ?? "") + "%"
            }).ToList();
        }

        public ShoesModel? GetShoe(int id)
        {
            using var db = Open();
            return db.QueryFirstOrDefault<ShoesModel>(@"
                SELECT s.id, s.name, s.description, s.price, s.image_url AS ImageUrl,
                       s.size, s.color, s.brand, s.category_id AS CategoryId, s.stock,
                       c.name AS CategoryName
                FROM shoes s
                JOIN categories c ON c.id = s.category_id
                WHERE s.id = @id", new { id });
        }

        public List<CategoryModel> GetCategories()
        {
            using var db = Open();
            return db.Query<CategoryModel>("SELECT id, name, description FROM categories ORDER BY name").ToList();
        }

        public List<string> GetBrands()
        {
            using var db = Open();
            return db.Query<string>("SELECT DISTINCT brand FROM shoes ORDER BY brand").ToList();
        }

        public void CreateShoe(ShoesModel shoe)
        {
            using var db = Open();
            db.Execute(@"
                INSERT INTO shoes (name, description, price, image_url, size, color, brand, category_id, stock)
                VALUES (@Name, @Description, @Price, @ImageUrl, @Size, @Color, @Brand, @CategoryId, @Stock)", shoe);
        }

        public void UpdateShoe(ShoesModel shoe)
        {
            using var db = Open();
            db.Execute(@"
                UPDATE shoes SET name=@Name, description=@Description, price=@Price, image_url=@ImageUrl,
                       size=@Size, color=@Color, brand=@Brand, category_id=@CategoryId, stock=@Stock
                WHERE id=@Id", shoe);
        }

        public void DeleteShoe(int id)
        {
            using var db = Open();
            db.Execute("DELETE FROM shoes WHERE id=@id", new { id });
        }

        public UserModel? FindUser(string usernameOrEmail)
        {
            using var db = Open();
            return db.QueryFirstOrDefault<UserModel>(@"
                SELECT id, username, email, password AS PasswordHash,
                       first_name AS FirstName, last_name AS LastName, role
                FROM users
                WHERE username = @value OR email = @value", new { value = usernameOrEmail });
        }

        public bool UsernameExists(string username)
        {
            using var db = Open();
            return db.ExecuteScalar<int>("SELECT COUNT(*) FROM users WHERE username=@username", new { username }) > 0;
        }

        public bool EmailExists(string email)
        {
            using var db = Open();
            return db.ExecuteScalar<int>("SELECT COUNT(*) FROM users WHERE email=@email", new { email }) > 0;
        }

        public void CreateUser(UserModel user)
        {
            using var db = Open();
            db.Execute(@"
                INSERT INTO users (username, email, password, first_name, last_name, role)
                VALUES (@Username, @Email, @PasswordHash, @FirstName, @LastName, @Role)", user);
        }

        public void EnsureAdmin()
        {
            using var db = Open();
            var exists = db.ExecuteScalar<int>("SELECT COUNT(*) FROM users WHERE username='admin'");
            if (exists == 0)
            {
                db.Execute(@"
                    INSERT INTO users (username, email, password, first_name, last_name, role)
                    VALUES ('admin', 'admin@kickzone.bg', @hash, 'Admin', 'KickZone', 'admin')",
                    new { hash = PasswordHasher.Hash("admin123") });
            }
        }

        public List<CartItemModel> GetCart(int userId)
        {
            using var db = Open();
            return db.Query<CartItemModel>(@"
                SELECT ci.id, ci.user_id AS UserId, ci.shoe_id AS ShoeId, ci.quantity AS Quantity,
                       s.name AS ShoeName, s.price AS ShoePrice, s.image_url AS ShoeImageUrl,
                       s.size AS ShoeSize, s.color AS ShoeColor, s.brand AS ShoeBrand, s.stock AS Stock
                FROM cart_items ci
                JOIN shoes s ON s.id = ci.shoe_id
                WHERE ci.user_id = @userId
                ORDER BY ci.id", new { userId }).ToList();
        }

        public int GetCartCount(int userId)
        {
            using var db = Open();
            return db.ExecuteScalar<int>(
                "SELECT COALESCE(SUM(quantity), 0) FROM cart_items WHERE user_id=@userId",
                new { userId });
        }

        public void AddToCart(int userId, int shoeId, int quantity = 1)
        {
            using var db = Open();
            db.Execute(@"
                INSERT INTO cart_items (user_id, shoe_id, quantity)
                VALUES (@userId, @shoeId, @quantity)
                ON DUPLICATE KEY UPDATE quantity = quantity + @quantity",
                new { userId, shoeId, quantity });
        }

        public void RemoveFromCart(int cartItemId, int userId)
        {
            using var db = Open();
            db.Execute("DELETE FROM cart_items WHERE id=@cartItemId AND user_id=@userId", new { cartItemId, userId });
        }

        public int CreateOrder(int userId, List<CartItemModel> items)
        {
            if (items.Count == 0)
                throw new InvalidOperationException("Количката е празна.");

            using var db = Open();
            using var tx = db.BeginTransaction();

            try
            {
                var total = items.Sum(i => i.ShoePrice * i.Quantity);

                db.Execute(@"
                    INSERT INTO orders (user_id, total_price, status)
                    VALUES (@userId, @total, 'pending')",
                    new { userId, total }, tx);

                var orderId = db.ExecuteScalar<int>("SELECT LAST_INSERT_ID()", transaction: tx);

                foreach (var item in items)
                {
                    db.Execute(@"
                        INSERT INTO order_items (order_id, shoe_id, quantity, unit_price)
                        VALUES (@orderId, @shoeId, @quantity, @unitPrice)",
                        new { orderId, shoeId = item.ShoeId, quantity = item.Quantity, unitPrice = item.ShoePrice }, tx);

                    var updated = db.Execute(
                        "UPDATE shoes SET stock = stock - @quantity WHERE id=@shoeId AND stock >= @quantity",
                        new { quantity = item.Quantity, shoeId = item.ShoeId }, tx);

                    if (updated == 0)
                        throw new InvalidOperationException("Няма достатъчно наличност за " + item.ShoeName + ".");
                }

                db.Execute("DELETE FROM cart_items WHERE user_id=@userId", new { userId }, tx);
                tx.Commit();
                return orderId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public int CountUsers() => Scalar("SELECT COUNT(*) FROM users");
        public int CountShoes() => Scalar("SELECT COUNT(*) FROM shoes");
        public int CountOrders() => Scalar("SELECT COUNT(*) FROM orders");

        public List<OrderModel> GetRecentOrders(int take = 10)
        {
            using var db = Open();
            return db.Query<OrderModel>(@"
                SELECT o.id, o.user_id AS UserId, o.total_price AS TotalPrice, o.status, o.created_at AS CreatedAt,
                       u.username AS Username
                FROM orders o
                JOIN users u ON u.id = o.user_id
                ORDER BY o.id DESC
                LIMIT @take", new { take }).ToList();
        }

        public List<DailyStatModel> GetRevenueByDay(int days = 14)
        {
            using var db = Open();
            return db.Query<DailyStatModel>(@"
                SELECT DATE(created_at) AS Day,
                       COUNT(*) AS Orders,
                       COALESCE(SUM(total_price), 0) AS Revenue
                FROM orders
                WHERE created_at >= CURDATE() - INTERVAL @days DAY
                GROUP BY DATE(created_at)
                ORDER BY Day", new { days }).ToList();
        }

        private int Scalar(string sql)
        {
            using var db = Open();
            return db.ExecuteScalar<int>(sql);
        }
    }
}
