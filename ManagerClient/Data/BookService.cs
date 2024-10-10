using System;
using System.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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

	public class Order
		{
    		public int OrderId { get; set; }
    		public DateTime OrderDateTime { get; set; }
    		public string Status { get; set; }
    		public string ShoppingAddress { get; set; }
    		public string UserName { get; set; }
    		public string BookName { get; set; }
    		public string Title { get; set; }
    		public decimal Price { get; set; }
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
		/*
			Mysql不支持单连接并发操作
			Mysql.Data也不支持队列 连接池
			为了方便直接一个查询一个连接

			关于释放：
				释放？什么释放？直接交给GC就行了
		*/
		public MySqlConnection GetConn()
		{
			GC.Collect(); // 因为每次打开后都不会删，所以干脆每次都主动要求回收一次

			MySqlConnection conn = new(mySqlConnectionStringBuilder.ConnectionString);
			conn.Open();

			return conn;
		}

        public BookService()
		{
            //conn = new(mySqlConnectionStringBuilder.ConnectionString);
            //conn.Open();
		}

		// 统计信息
		public int GetUserCount()
		{
            MySqlCommand UserCountCmd = new("select count(*) from Users", GetConn());
			var reader = UserCountCmd.ExecuteReader();
			reader.Read();
			int ret = reader.GetInt32(0);
			reader.Close();
			return ret;
		}

		public int GetIncome()
		{
			using var conn = GetConn();
			using var cmd = new MySqlCommand(@"
				select sum(price)
				from Orders
				join app_database.Books B on B.book_id = Orders.book_id
			",conn);

			return Convert.ToInt32(cmd.ExecuteScalar());
		}

		// 仓库书本操作
		public async Task<List<Book>> GetAllBooks()
		{
			List<Book> books = [];

			string query = "SELECT * FROM Books";
        	using (var cmd = new MySqlCommand(query, GetConn()))
        	{
            	using (var reader = await cmd.ExecuteReaderAsync())
            	{
                	while (await reader.ReadAsync())
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

		// 订单管理
		

		 public async Task<List<Order>> GetOrdersAsync()
    	{
        	var orders = new List<Order>();

        	using (var connection = GetConn())
        	{

            	string query = @"
                SELECT o.order_id, o.order_datetime, o.status, o.shopping_address, 
                       u.username, b.book_name, b.title, b.price
                FROM app_database.Orders o
                LEFT JOIN app_database.Users u ON o.user_id = u.user_id
                LEFT JOIN app_database.Books b ON o.book_id = b.book_id;
           	 	";

            	using (var command = new MySqlCommand(query, connection))
            	{
                	using (var reader = await command.ExecuteReaderAsync())
                	{
                    	while (await reader.ReadAsync())
                    	{
                        	var order = new Order() {
                            	OrderId = reader.GetInt32("order_id"),
                            	OrderDateTime = reader.GetDateTime("order_datetime"),
                            	Status = reader.GetString("status"),
                            	ShoppingAddress = reader.GetString("shopping_address"),
                            	UserName = reader.GetString("username"),
                            	BookName = reader.GetString("book_name"),
                            	Title = reader.GetString("title"),
                            	Price = reader.GetDecimal("price")
                        	};
                        orders.Add(order);
                    }
                }
            }
        	}

        	return orders;
    	}

	}
}

