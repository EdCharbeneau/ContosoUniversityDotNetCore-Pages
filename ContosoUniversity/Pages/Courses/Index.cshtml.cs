using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Pages.Courses
{
    public class Index : PageModel
    {
        private readonly IMediator _mediator;

        public Index(IMediator mediator) => _mediator = mediator;

        public Result Data { get; private set; }

        public void OnGetAsync() { }
        public async Task<JsonResult> OnGetCourses_Read([DataSourceRequest]DataSourceRequest request) => new JsonResult(await _mediator.Send(new Query(request)));

        public class Query : IRequest<DataSourceResult>
        {
            public Query(DataSourceRequest request) => dataSourceRequest = request;
            public DataSourceRequest dataSourceRequest { get; set; }
        }

        public class Result
        {
            //public DataSourceResult Courses { get; set; }

            public class Course
            {
                public int Id { get; set; }
                public string Title { get; set; }
                public int Credits { get; set; }
                public string DepartmentName { get; set; }
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Course, Result.Course>();
        }

        public class Handler : IRequestHandler<Query, DataSourceResult>
        {
            private readonly SchoolContext _db;
            private readonly IConfigurationProvider _configuration;

            public Handler(SchoolContext db, IConfigurationProvider configuration)
            {
                _db = db;
                _configuration = configuration;
            }

            public async Task<DataSourceResult> Handle(Query message, CancellationToken token)
            {
                var courses = await _db.Courses.ProjectTo<Result.Course>(_configuration).ToDataSourceResultAsync(message.dataSourceRequest);

                return courses;
            }
        }
    }
}