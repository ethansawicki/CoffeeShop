using CoffeeShop.Models;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;

namespace CoffeeShop.Repositories
{
    public class CoffeeRepository : ICoffeeRepository
    {
        private readonly string _connectionString;

        public CoffeeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private SqlConnection Connection
        {
            get { return new SqlConnection(_connectionString); }
        }

        public List<Coffee> GetAll()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Coffee.Id, Title, BeanVariety.Name as BeanVariety FROM Coffee JOIN BeanVariety on Coffee.BeanVarietyId = BeanVariety.Id GROUP BY Coffee.Id, Coffee.Title, BeanVariety.Name";
                    var reader = cmd.ExecuteReader();
                    var coffeeList = new List<Coffee>();
                    while (reader.Read())
                    {
                        var coffee = new Coffee()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),

                            beanVariety = new BeanVariety()
                            {
                                Name = reader.GetString(reader.GetOrdinal("BeanVariety")),
                            }
                        };
                        coffeeList.Add(coffee);
                    }
                    reader.Close();
                    return coffeeList;
                }
            }
        }

        public Coffee GetById(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Coffee.Id, Title, BeanVariety.Name as BeanVariety FROM Coffee JOIN BeanVariety on Coffee.BeanVarietyId = BeanVariety.Id AND Coffee.Id = @id GROUP BY Coffee.Id, Coffee.Title, BeanVariety.Name";
                    cmd.Parameters.AddWithValue("@id", id);

                    var reader = cmd.ExecuteReader();

                    Coffee coffeeVariety = null;
                    if (reader.Read())
                    {
                        coffeeVariety = new Coffee()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            beanVariety = new BeanVariety()
                            {
                                Name = reader.GetString(reader.GetOrdinal("BeanVariety")),
                            }
                        };
                    }
                    reader.Close();
                    return coffeeVariety;
                }
            }
        }

        public void Add(Coffee coffee)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO Coffee (Title, BeanVarietyId)
                        OUTPUT INSERTED.ID
                        VALUES (@title, @beanvarietyid)";
                    cmd.Parameters.AddWithValue("@title", coffee.Title);
                    cmd.Parameters.AddWithValue("@beanvarietyid", coffee.beanVariety.Id);
                    coffee.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void Update(Coffee coffee)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE Coffee
                            SET Title = @title,
                                BeanVarietyId = @beanvarietyid
                        WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", coffee.Id);
                    cmd.Parameters.AddWithValue("@title", coffee.Title);
                    cmd.Parameters.AddWithValue("@beanvarietyid", coffee.beanVariety.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Delete(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Coffee WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
