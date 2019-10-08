# NCoreCoder.Framework
> Author NCoreCode
>> [My Blog](https://www.cnblogs.com/NCoreCoder/)

# Develoment Tasks
* 2019/9/9
*  Add Aop(Aspect Oriented Programming)
[Sample](https://www.cnblogs.com/NCoreCoder/p/11492755.html)
* 2019/9/10
* Add propety inject, Update Reflection for [AspectCore.Extensions.Reflection](https://github.com/dotnetcore/AspectCore-Framework)
* 2019/9/23
* Add Emit Build Aop Proxy class [Sample](https://www.cnblogs.com/NCoreCoder/p/11572463.html)
* 2019/9/25
* Add Support .Net Core 3.0
* Add Auto Inject Aop
* [Sample](https://www.cnblogs.com/NCoreCoder/p/11586797.html)
* 2019/9/26
* Add Custom Aop Actors
* 2019/10/8
* Attribute Edit, method contains After、AfterAsync, Add Paramater Execption
* IAopActors and DefaultActors Edit, All Add Paramater Func Delegate
* [Sample](https://www.cnblogs.com/NCoreCoder/p/11634642.html)
#Sample
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
method Hello add default aop, but custom, add attribute inherit JitAopAttribute
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
Edit TestClass
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

Use TestAopActors
```csharp
    [AopActors(typeof(TestActors))]
    public interface ITestClass
    {
        string Hello();
        Task<int> ResultIntAsync();
        Task ReturnAsync();
    }
```
