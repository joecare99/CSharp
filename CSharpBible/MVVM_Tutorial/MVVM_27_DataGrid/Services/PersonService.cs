using MVVM_27_DataGrid.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_27_DataGrid.Services
{
    public class PersonService
    {
        public IEnumerable<Person> GetPersons()
        {
            var deps = GetDepartments();
            //var 
            return new Person[]
            {
                    new(){FirstName="Max",LastName="Muster",Email="max@muster.com",Id=1,Department = deps[0] },
                    new(){FirstName="Susi",LastName="Müller",Email="susi@muster.com",Id=2,Department = deps[1],Birthday= new DateTime(1980,1,1) },
                    new(){FirstName="Dave",LastName="Dev",Email="dev.dave@muster.com",Id=3,Department = deps[3],Birthday= new DateTime(1988,5,2) },
                    new(){FirstName="Herbert",LastName="Bossinger",Email="ceo@muster.com",Id=4,Department = deps[2],Birthday= new DateTime(1999,7,7) }
            };
        }
        public Department[] GetDepartments() 
            => new Department[]{
                new(){ Id=1,Name="Engineering",Description="The Engineering-department" },
                new(){ Id=2,Name="Sales",Description="The Sales-department" },
                new(){ Id=3,Name="Management",Description="The Management" },
                new(){ Id=4,Name="Service",Description="The Service-department" } }
                ;
    }
}
