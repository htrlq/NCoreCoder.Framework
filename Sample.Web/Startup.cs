using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCoreCoder.Aop;

namespace Sample.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddNCoreCoderAop<ITestProxy, TestProxy>(ServiceLifetime.Transient);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }

    public interface ITestProxy
    {
        [Test]
        void Test();

        [Test]
        int TestInt();

        [Test]
        Task TestAsync();

        [Test]
        Task<int> ResultAsync();
    }

    public class TestProxy: ITestProxy
    {
        public void Test()
        {
            Console.WriteLine("test");
        }

        public int TestInt()
        {
            Console.WriteLine("TestInt");
            return 1000;
        }

        public async Task TestAsync()
        {
            Console.WriteLine("TestAsync");
            await Task.Delay(5000);
            Console.WriteLine("TestAsync Delay 5000");

            await Task.CompletedTask;
        }

        public async Task<int> ResultAsync()
        {
            Console.WriteLine("ResultAsync");
            await Task.Delay(5000);
            Console.WriteLine("ResultAsync Delay 5000");

            return 900;
        }
    }

    public class TestAttribute: AopAttribute
    {
        protected override Task BeforeAsync(MethodInfo targetMethod, object[] args)
        {
            Console.WriteLine("Test Before Async");

            return base.BeforeAsync(targetMethod, args);
        }

        protected override Task AfterAsync(MethodInfo targetMethod, object[] args)
        {
            Console.WriteLine("Test After Async");

            return base.AfterAsync(targetMethod, args);
        }
    }
}
