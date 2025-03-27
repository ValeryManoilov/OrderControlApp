
using LanguageExt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrderControlApp.Context;
using OrderControlApp.Dto;
using OrderControlApp.Managers;
using OrderControlApp.Models;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Data.Sqlite;
namespace OrderControlApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Either<string, NameDto> ValidateInputName(string inputName)
            {
                if (inputName.Length < 6)
                {
                    return "Слишком короткое имя (название). Оно должно быть не меньше 6)";
                }

                if (inputName.Length > 20)
                {
                    return "Слишком длинное имя (название). Оно должно быть не больше 20";
                }

                return new NameDto { Name = inputName };
            }

            Either<string, int> ValidateInputPrice(string price)
            {
                if (!Int32.TryParse(price, out int newPrice))
                {
                    return "Цена должна быть числом!";
                }

                if (newPrice <= 0)
                {
                    return "Цена должна быть натуральными числом (больше ноля)";
                }

                return newPrice;
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.InputEncoding = Encoding.GetEncoding(866);
            Console.OutputEncoding = Encoding.GetEncoding(866);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var services = new ServiceCollection();

            // Register DbContext with configuration

            services.AddLogging();

            services.AddDbContext<OrderControllAppContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("connectionString")));

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
            }
            )
            .AddEntityFrameworkStores<OrderControllAppContext>()
            .AddDefaultTokenProviders()
            .AddUserManager<UserManager<User>>()
            .AddSignInManager<SignInManager<User>>();

            services.AddScoped<ClientManager>();
            services.AddScoped<OrderManager>();
            services.AddScoped<ProductManager>();
            services.AddScoped<AccountManager>();

            services.AddAuthentication();
            services.AddAuthorization();

            var serviceProvider = services.BuildServiceProvider();

            var scope = serviceProvider.CreateScope();


            var dbContext = scope.ServiceProvider.GetRequiredService<OrderControllAppContext>();

            var accounts = scope.ServiceProvider.GetRequiredService<AccountManager>();
            var clients = scope.ServiceProvider.GetRequiredService<ClientManager>();
            var orders = scope.ServiceProvider.GetRequiredService<OrderManager>();
            var products = scope.ServiceProvider.GetRequiredService<ProductManager>();
            await dbContext.Database.EnsureCreatedAsync();


            bool flag1 = true;

            bool flagReg = true;

            Console.WriteLine("Приветствую тебя в админ-панели приложения по управлению заказами!");


            while (flagReg)
            {
                Console.WriteLine("\n1. Регистрация");
                Console.WriteLine("2. Вход\n");
                Console.WriteLine("3. Выйти");

                Console.WriteLine("\nВыберите вариант входа:");
                string ans = Console.ReadLine().Trim();

                while (!Decimal.TryParse(ans, out decimal result))
                {
                    Console.WriteLine("Введите число!");
                    ans = Console.ReadLine();
                }

                switch (Int32.Parse(ans))
                {
                    case 1:

                        Console.WriteLine("\nВведите имя:");

                        string name1 = Console.ReadLine();

                        Console.WriteLine("Введите пароль:");

                        string password1 = Console.ReadLine();

                        CreateUserDto createUserData = new CreateUserDto
                        {
                            Name = name1,
                            Password = password1
                        };

                        var result = await accounts.RegistrationAsync(createUserData);

                        if (result.IsLeft)
                        {
                            Console.WriteLine($"\n{(string)result}\n");
                        }
                        else
                        {
                            Console.WriteLine("\nВы успешно зарегистрировались!\n");

                            flagReg = false;
                        }

                        break;

                    case 2:

                        Console.WriteLine("Введите имя:");

                        string name2 = Console.ReadLine();

                        Console.WriteLine("Введите пароль:");

                        string password2 = Console.ReadLine();

                        CreateUserDto loginUserData = new CreateUserDto
                        {
                            Name = name2,
                            Password = password2
                        };

                        var result2 = await accounts.LoginAsync(loginUserData);

                        if (result2)
                        {
                            Console.WriteLine("Вы успешно авторизовались");

                            flagReg = false;
                        }
                        else
                        {
                            Console.WriteLine("Не удалось войти");
                        }

                        break;

                    case 3:

                        flag1 = false; flagReg = false;

                        break;

                    default:

                        Console.WriteLine("Введите число!");

                        break;
                }
            }

            while (flag1)
            {
                Console.WriteLine("\n^&*^&*^&*^&*^&*^&*^&*^&*^&*^*^&*^&*^&*^&*^*&^&*^*&^&*\n");

                Console.WriteLine("1. Добавить клиента");
                Console.WriteLine("2. Добавить товар");
                Console.WriteLine("3. Создать заказ");
                Console.WriteLine("4. Вывести все заказы клиента");
                Console.WriteLine("5. Вывести все товары, заказанный в определенный период времени");
                Console.WriteLine("6. Выход");

                Console.WriteLine("\nДополнительные функции:\n");
                
                Console.WriteLine("7. Вывести всех клиентов");
                Console.WriteLine("8. Вывести все товары");
                Console.WriteLine("9. Вывести все заказы");



                Console.WriteLine("\nВыберите нужную функцию:");
                string ans = Console.ReadLine().Trim();

                while (!Decimal.TryParse(ans, out decimal result))
                {
                    Console.WriteLine("Введите число!");
                    ans = Console.ReadLine();
                }

                switch (Int32.Parse(ans))
                {
                    case 1:


                        Console.WriteLine("\nВведите имя клиента");

                        var name1 = ValidateInputName(Console.ReadLine().Trim());

                        while (name1.IsLeft)
                        {
                            Console.WriteLine((string)name1);

                            name1 = ValidateInputName(Console.ReadLine().Trim());
                        }


                        Console.WriteLine("\nВведите почту");

                        string email1 = Console.ReadLine().Trim();




                        Console.WriteLine("\nВведите номер телефона");

                        string phone1 = Console.ReadLine().Trim();

                        AddClientDto newClientData = new AddClientDto
                        {
                            Name = ((NameDto)name1).Name,
                            Email = email1,
                            Phone = phone1
                        };

                        var result = await clients.AddClientAsync(newClientData);

                        Console.WriteLine(result);

                        break;

                    case 2:

                        Console.WriteLine("\nВведите название товара");

                        var title1 = ValidateInputName(Console.ReadLine().Trim());

                        while (title1.IsLeft)
                        {
                            Console.WriteLine((string)title1);

                            title1 = ValidateInputName(Console.ReadLine().Trim());
                        }


                        Console.WriteLine("\nВведите цену");

                        var price1 = ValidateInputPrice(Console.ReadLine().Trim());

                        while (price1.IsLeft)
                        {
                            Console.WriteLine((string)price1);

                            price1 = ValidateInputPrice(Console.ReadLine().Trim());
                        }


                        AddProductDto newProductData = new AddProductDto
                        {
                            Name = ((NameDto)title1).Name,
                            Price = (decimal)price1
                        };

                        var result2 = await products.AddProductAsync(newProductData);

                        Console.WriteLine(result2);

                        break;

                    case 3:

                        Console.WriteLine("\nВведите имя клиента");

                        string name3 = Console.ReadLine().Trim();

                        List<AddProductToOrderDto> addingProducts = new List<AddProductToOrderDto>();


                        while (true)
                        {
                            Console.WriteLine("\nВведите название товара. Если хотите прекратить ввод, напишите 'закончить'");

                            string productName = Console.ReadLine().Trim();

                            if (productName.ToLower() == "закончить")
                            {
                                break;
                            }

                            Console.WriteLine("\nВведите количество товара:");

                            int quantity = Convert.ToInt32(Console.ReadLine());

                            AddProductToOrderDto addProductDto = new AddProductToOrderDto
                            {
                                ProductName = productName,
                                Quantity = quantity
                            };

                            addingProducts.Add(addProductDto);
                        }

                        CreateOrderDto creatingOrder = new CreateOrderDto
                        {
                            ClientName = name3,
                            ProductsData = addingProducts
                        };

                        var result3 = await orders.CreateOrderAsync(creatingOrder);

                        Console.WriteLine(result3);

                        break;

                    case 4:

                        Console.WriteLine("\nВведите имя клиента");

                        string name4 = Console.ReadLine().Trim();

                        var result4 = await orders.GetClientOrdersAsync(name4);

                        if (result4.IsLeft)
                        {
                            Console.WriteLine((string)result4);
                        }

                        else
                        {

                            List<GetOrderDto> needOrders = ((List<GetOrderDto>)result4);
                            
                            foreach (var order in needOrders)
                            {
                                Console.WriteLine($"\n-------- Заказ {order.Id} ------------\n");

                                Console.WriteLine($"Имя клиента: {order.Name}\n");

                                Console.WriteLine($"Время заказа: {order.OrderDate.Hour:00}:{order.OrderDate.Minute:00} " +
                                    $"{order.OrderDate.Day}.{order.OrderDate.Month}.{order.OrderDate.Year}\n");

                                Console.WriteLine($"Стоимость заказа: {order.TotalPrice}\n");

                                Console.WriteLine("Купленные товары:\n");

                                Console.WriteLine("Название - цена - количество\n");
                                foreach (var needProduct in order.Products)
                                {
                                    Console.WriteLine($"{needProduct.Name} - {needProduct.Price} - {needProduct.Quantity}");
                                }
                            }
                        }


                        break;

                    case 5:

                        Console.WriteLine("\nВведите день начала периода");

                        int startDay = Int32.Parse(Console.ReadLine().Trim());

                        Console.WriteLine("\nВведите месяц начала периода");

                        int startMonth = Int32.Parse(Console.ReadLine().Trim());

                        Console.WriteLine("\nВведите год начала периода");

                        int startYear = Int32.Parse(Console.ReadLine().Trim());


                        Console.WriteLine("\nВведите день конца периода");

                        int endDay = Int32.Parse(Console.ReadLine().Trim());

                        Console.WriteLine("\nВведите месяц конца периода");

                        int endMonth = Int32.Parse(Console.ReadLine().Trim());

                        Console.WriteLine("\nВведите год конца периода");

                        int endYear = Int32.Parse(Console.ReadLine().Trim());

                        DateTimeDto startDate = new DateTimeDto
                        {
                            Year = startYear,
                            Month = startMonth,
                            Day = startDay
                        };

                        DateTimeDto endDate = new DateTimeDto
                        {
                            Year = endYear,
                            Month = endMonth,
                            Day = endDay
                        };

                        DateFrameDto dateFrame = new DateFrameDto
                        {
                            StartDate = startDate,
                            EndDate = endDate,
                        };


                        var result5 = await products.GetProductByDateAsync(dateFrame);

                        if (result5.IsLeft)
                        {
                            Console.WriteLine((string)result5);
                        }
                        else
                        {
                            List<GetProductDto> needProducts = (List<GetProductDto>)result5;

                            Console.WriteLine("\nВремя заказа - клиент - продукт - цена - количество");

                            foreach (var product in needProducts)
                            {
                                Console.WriteLine($"{product.OrderDate.Hour:00}:{product.OrderDate.Minute:00} " +
                                    $"{product.OrderDate.Day:00}.{product.OrderDate.Hour:00}.{product.OrderDate.Year} " +
                                    $"- {product.ClientName} - {product.ProductName} - {product.Price} - {product.Quantity}");
                            }
                        }

                        break;

                    case 6:

                        flag1 = false;

                        break;

                    case 7:

                        List<GetClientDto> allClients = await clients.GetAllClientsAsync();

                        Console.WriteLine("\nId - имя - почта - телефон");

                        foreach(var client in allClients)
                        {
                            Console.WriteLine($"{client.Id} - {client.Name} - {client.Email} - {client.Phone}");
                        }

                        break;

                    case 8:

                        List<GetFormatProductDto> allProducts = await products.GetAllProductsAsync();

                        Console.WriteLine("\nId - название - цена");

                        foreach(var product in allProducts)
                        {
                            Console.WriteLine($"{product.Id} - {product.Name} - {product.Price}");
                        }

                        break;

                    case 9:

                        List<GetOrderDto> allOrders = await orders.GetAllOrdersAsync();

                        foreach (var order in allOrders)
                        { 
                            Console.WriteLine($"\n-------- Заказ {order.Id} ------------\n");

                            Console.WriteLine($"Имя клиента: {order.Name}\n");

                            Console.WriteLine($"Время заказа: {order.OrderDate.Hour:00}:{order.OrderDate.Minute:00} " +
                                $"{order.OrderDate.Day}.{order.OrderDate.Month}.{order.OrderDate.Year}\n");

                            Console.WriteLine($"Стоимость заказа: {order.TotalPrice}\n");

                            Console.WriteLine("Купленные товары:\n");

                            Console.WriteLine("Название - цена - количество\n");
                            foreach (var needProduct in order.Products)
                            {
                                Console.WriteLine($"{needProduct.Name} - {needProduct.Price} - {needProduct.Quantity}");
                            }
                        }

                        break;


                    default:

                        Console.WriteLine("Введите цифру");

                        break;
                }
            }




            //var builder = WebApplication.CreateBuilder(args);

            //// Add services to the container.

            //builder.Services.AddControllers();
            //builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();

            //builder.Services.AddDbContext<OrderControllAppContext>();

            //builder.Services.AddScoped<ClientManager>();
            //builder.Services.AddScoped<OrderManager>();
            //builder.Services.AddScoped<ProductManager>();

            //var app = builder.Build();

            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            //app.UseHttpsRedirection();

            //app.UseAuthorization();


            //app.MapControllers();

            //app.Run();
        }
    }
}
