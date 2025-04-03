using Email_Worker2.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Email_Worker2.DataRepository
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
               serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            context.Users.AddRange(
              new User
              {
                  Name = "Ayo",
                  Email = "ayomidotun.odebode@elizadeuniversity.edu.ng",
                  IsEmailSent = false
              },
              new User
              {
                  Name = "Mayowa",
                  Email = "adedaramayowa6@gmail.com",
                  IsEmailSent = false
              },
              new User
              {
                  Name = "ituah",
                  Email = "vibezr35@gmail.com",
                  IsEmailSent = false
              },
              new User
              {
                  Name = "dotun",
                  Email = "ayomidotun.odebode1@gmail.com",
                  IsEmailSent = false
              },
               new User
               {
                   Name = "Mayor",
                   Email = "adedarajesse@gmail.com",
                   IsEmailSent = false
               }
          );

            await context.SaveChangesAsync();
        }
    }
}
