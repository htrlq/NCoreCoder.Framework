# NCoreCoder.Framework
> 作者 NCoreCode
>> [我的博客](https://www.cnblogs.com/NCoreCoder/)

# 开发任务
* 2019/9/9
* 增加AOP(Aspect Oriented Programming，面向切面编程)
* [例子](https://www.cnblogs.com/NCoreCoder/p/11492755.html)
* 2019/9/10
* 增加属性注入, 使用反射优化 [AspectCore.Extensions.Reflection](https://github.com/dotnetcore/AspectCore-Framework)
* 2019/9/23
* 增加Emit代理AOP（之前的代理是基于DispatchProxy，DispatchProxy不支持构造器注入，才增加的属性注入） 
* [例子](https://www.cnblogs.com/NCoreCoder/p/11572463.html)
* 2019/9/25
* 增加Asp.Net Core 3.0支持，[原理参考](https://www.cnblogs.com/NCoreCoder/p/11641773.html)
* 增加自动注入AOP功能
* [例子](https://www.cnblogs.com/NCoreCoder/p/11586797.html)
* 2019/9/26
* 增加自定义AOP执行器
* 2019/10/8
* 特性修改, 对After、AfterAsync方法增加了一个处理异常的参数
* 对于IAopActors、DefaultActors的所有参数，增加了一个委托回调的参数
* 修复目标类重名问题
* [例子](https://www.cnblogs.com/NCoreCoder/p/11634642.html)
* 2019/10/10
* 代理类增加了属性克隆支持
* 增加ValueTask支持
* 增加双版本支持 .Net Standard 2.0\.Net Framework 4.5

# Code Sample
```csharp
    //new interface
    public interface ITestClass
    {
        string Hello();
        Task<int> ResultIntAsync();
        Task ReturnAsync();
    }
    
    //new class inherit ITestClass
    [JitInject]
    public class TestClass: ITestClass
    {
        [JitAop]
        public string Hello()
        {
            Console.WriteLine("Hello");
            return "Hello";
        }

        public Task<int> ResultIntAsync()
        {
            Console.WriteLine("ResultIntAsync");
            return Task.FromResult(100);
        }

        public Task ReturnAsync()
        {
            Console.WriteLine("ReturnAsync");
            return Task.CompletedTask;
        }
    }
```
## class TestClass add aop
method "Hello" add default aop, but custom, add attribute inherit JitAopAttribute
support async method and sync method
```csharp
    [AttributeUsage(AttributeTargets.Method)]
    internal class TestJitAttribute : JitAopAttribute
    {
        public override void Before(MethodReflector method, object instance, params object[] param)
        {
            Console.WriteLine($"Before Name:{method.Name}");
        }

        public override void After(MethodReflector method, Exception exception, object instance, params object[] param)
        {
            Console.WriteLine($"After Name:{method.Name}");
        }

        public override Task BeforeAsync(MethodReflector method, object instance, params object[] param)
        {
            Console.WriteLine($"BeforeAsync Name:{method.Name}");
            return Task.CompletedTask;
        }

        public override Task AfterAsync(MethodReflector method, Exception exception, object instance, params object[] param)
        {
            Console.WriteLine($"AfterAsync Name:{method.Name}");
            return Task.CompletedTask;
        }
    }
```
### Edit TestClass
```csharp
    [JitInject]
    public class TestClass: ITestClass
    {
        [TestJit]
        public string Hello()
        {
            Console.WriteLine("Hello");
            return "Hello";
        }
        
        ///...
    }
```
# cutom Actor
```csharp
    public class TestActors : IAopActors
    {
        public object Execute(AopContext context)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> ExecuteAsync<TResult>(AopContext context)
        {
            throw new NotImplementedException();
        }

        public Task InvokeAsync(AopContext context)
        {
            throw new NotImplementedException();
        }
    }
```

## Use TestAopActors
```csharp
    [AopActors(typeof(TestActors))]
    public interface ITestClass
    {
        string Hello();
        Task<int> ResultIntAsync();
        Task ReturnAsync();
    }
```
# Run
## Under Asp.net Core 3.0
edit Startup.cs
```csharp
   public IServiceProvider ConfigureServices(IServiceCollection services)
   {
      //...
      services.AddSingleton<ITestClass, TestClass>();
      return services.BuilderJit();
   }
```
# Asp.net Core 3.0
edit Program.cs
```csharp
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new JitServiceProviderFactory()) //new
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
```
edit Startup.cs
add Method
```csharp
        public void ConfigureContainer(JitAopBuilder builder)
        {
            builder.Add<ITestClass, TestClass>(ServiceLifetime.Singleton);
        }
```
