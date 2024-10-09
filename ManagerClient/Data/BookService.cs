using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace ManagerClient.Data
{
	public class Book
	{
    	public int BookId { get; set; }
    	public string Title { get; set; }
    	public string Description { get; set; }
    	public string Author { get; set; }
    	public decimal Price { get; set; }
    	public byte[] Image { get; set; } // 用于存储图像
    	public string BookName { get; set; }
	}
	public class BookService
	{
		MySqlConnectionStringBuilder mySqlConnectionStringBuilder = new MySqlConnectionStringBuilder()
		{
			Server = "121.36.201.254",
			Port = 3306,
			UserID = "root",
			Password = "w+uf2,hm9n%E",
			Database = "app_database"
        };

		//MySqlConnection conn;

		public MySqlConnection GetConn()
		{
			MySqlConnection conn = new(mySqlConnectionStringBuilder.ConnectionString);
			conn.Open();

			return conn;
		}

        public BookService()
		{
            //conn = new(mySqlConnectionStringBuilder.ConnectionString);
            //conn.Open();
		}

		public int GetUserCount()
		{
            MySqlCommand UserCountCmd = new("select count(*) from Users", GetConn());
			var reader = UserCountCmd.ExecuteReader();
			reader.Read();
			int ret = reader.GetInt32(0);
			reader.Close();
			return ret;
		}
		// 仓库书本操作
		public List<Book> GetAllBooks()
		{
			List<Book> books = new List<Book>();

			string query = "SELECT * FROM Books";
        	using (var cmd = new MySqlCommand(query, GetConn()))
        	{
            	using (var reader = cmd.ExecuteReader())
            	{
                	while (reader.Read())
                	{
                    	books.Add(new Book
                    	{
                        	BookId = reader.GetInt32("book_id"),
                        	Title = reader.GetString("title"),
                        	Description = reader.IsDBNull("description") ? null : reader.GetString("description"),
                        	Author = reader.GetString("author"),
                        	Price = reader.GetDecimal("price"),
                        	Image = reader.IsDBNull("image") ? null : (byte[])reader["image"],
                        	BookName = reader.IsDBNull("book_name") ? null : reader.GetString("book_name")
                    	});
                	}
            	}
        	}

			return books;
		}

		public void AddBook(Book newBook) {

        	string query = "INSERT INTO Books (title, description, author, price, image, book_name) " +
                       "VALUES (@Title, @Description, @Author, @Price, @Image, @BookName)";

        	using (var cmd = new MySqlCommand(query, GetConn()))
        	{
            		cmd.Parameters.AddWithValue("@Title", newBook.Title);             // 图书类型
            		cmd.Parameters.AddWithValue("@Description", newBook.Description); // 图书描述
            		cmd.Parameters.AddWithValue("@Author", newBook.Author);           // 作者
            		cmd.Parameters.AddWithValue("@Price", newBook.Price);             // 价格
            		cmd.Parameters.AddWithValue("@Image", newBook.Image);             // 图片数据
            		cmd.Parameters.AddWithValue("@BookName", newBook.BookName);       // 书名

            		cmd.ExecuteNonQuery();
        	}
    	}

		public void UpdateBook(Book updatedBook){
			string query = "UPDATE Books SET title = @Title, description = @Description, author = @Author, " +
                       "price = @Price, image = @Image, book_name = @BookName WHERE book_id = @BookId";

        	using (var cmd = new MySqlCommand(query, GetConn()))
        	{
            	cmd.Parameters.AddWithValue("@Title", updatedBook.Title);             // 图书类型
            	cmd.Parameters.AddWithValue("@Description", updatedBook.Description); // 图书描述
            	cmd.Parameters.AddWithValue("@Author", updatedBook.Author);           // 作者
            	cmd.Parameters.AddWithValue("@Price", updatedBook.Price);             // 价格
            	cmd.Parameters.AddWithValue("@Image", updatedBook.Image);             // 图片数据
            	cmd.Parameters.AddWithValue("@BookName", updatedBook.BookName);       // 书名
           	 	cmd.Parameters.AddWithValue("@BookId", updatedBook.BookId);           // 书本ID

            	cmd.ExecuteNonQuery();
        	}
		}

		public void DeleteBook(int bookId)
		{
			 string query = "DELETE FROM Books WHERE book_id = @BookId";

        	using (var cmd = new MySqlCommand(query, GetConn()))
        	{
           		cmd.Parameters.AddWithValue("@BookId", bookId); // 书本ID
            	cmd.ExecuteNonQuery();
        	}
		}
	}
}

