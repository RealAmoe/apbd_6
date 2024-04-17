using Microsoft.AspNetCore.Mvc;
using Tutorial5.Models;
using Microsoft.Data.SqlClient;


namespace Tutorial5.Controllers
{
    [ApiController]
    [Route("api/animals")]
    public class AnimalsController : ControllerBase
    {
        private const string ConnString = "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True;Trust Server Certificate=True;";

        [HttpGet]
        public IActionResult GetAnimals([FromQuery] string orderBy = "name")
        {
            var validSortByValues = new HashSet<string> {"name", "description", "category", "area"};
            orderBy = validSortByValues.Contains(orderBy.ToLower()) ? orderBy : "name";

            var animals = new List<Animal>();
            try
            {
                using (var conn = new SqlConnection(ConnString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand($"SELECT * FROM Animal ORDER BY {orderBy}", conn))
                    {
                        using (var dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                animals.Add(new Animal
                                {
                                    IdAnimal = (int)dr["IdAnimal"],
                                    Name = dr["Name"].ToString(),
                                    Description = dr["Description"].ToString(),
                                    Category = dr["Category"].ToString(),
                                    Area = dr["Area"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                return StatusCode(500, "Error accessing the database: " + e.Message + e.StackTrace);
            }
            return Ok(animals);
        }

        [HttpPost]
        public IActionResult AddAnimal(string name, string description, string category, string area)
        {
            try
            {
                using (var conn = new SqlConnection(ConnString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("INSERT INTO Animal (Name, Description, Category, Area) VALUES (@name, @desc, @cat, @area)", conn);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@desc", description);
                    cmd.Parameters.AddWithValue("@cat", category);
                    cmd.Parameters.AddWithValue("@area", area);
                    cmd.ExecuteNonQuery();
                }
                return StatusCode(201); // Created
            }
            catch (SqlException e)
            {
                return StatusCode(500, "Error adding the animal to the database: " + e.Message);
            }
        }

        [HttpPut("{idAnimal}")]
        public IActionResult UpdateAnimal(int idAnimal, Animal animal)
        {
            try
            {
                using (var conn = new SqlConnection(ConnString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("UPDATE Animal SET Name = @name, Description = @desc, Category = @cat, Area = @area WHERE IdAnimal = @id", conn);
                    cmd.Parameters.AddWithValue("@name", animal.Name);
                    cmd.Parameters.AddWithValue("@desc", animal.Description);
                    cmd.Parameters.AddWithValue("@cat", animal.Category);
                    cmd.Parameters.AddWithValue("@area", animal.Area);
                    cmd.ExecuteNonQuery();
                }
                return NoContent(); // 204 No Content
            }
            catch (SqlException e)
            {
                return StatusCode(500, "Error updating the animal in the database: " + e.Message);
            }
        }

        [HttpDelete("{idAnimal}")]
        public IActionResult DeleteAnimal(int idAnimal)
        {
            try
            {
                using (var conn = new SqlConnection(ConnString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("DELETE FROM Animal WHERE IdAnimal = @id", conn);
                    cmd.Parameters.AddWithValue("@id", idAnimal);
                    cmd.ExecuteNonQuery();
                }
                return NoContent(); // 204 No Content
            }
            catch (SqlException e)
            {
                return StatusCode(500, "Error deleting the animal from the database: " + e.Message);
            }
        }
    }
}
