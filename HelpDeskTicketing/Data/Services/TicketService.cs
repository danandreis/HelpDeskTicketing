using AutoMapper;
using HelpDeskTicketing.Data.ViewModels;
using HelpDeskTicketing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace HelpDeskTicketing.Data.Services
{
    public class TicketService : ITicketService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPrincipal _principal;
        private readonly UserManager<AppUser> _userManager;

        public TicketService(AppDbContext context,IMapper mapper, IPrincipal principal, UserManager<AppUser> userManager)
        {

            _context = context;
            _mapper = mapper;
            _principal = principal;
            _userManager = userManager;
        }

        public async Task<IEnumerable<Ticket>> GetTicketsList(string userName)
        {

            /*var ticketsList = from t in _context.Tickets
                              join d in _context.Domains on t.DomainId equals d.Id
                              join s in _context.Statuses on t.StatusId equals s.Id
                              join b in _context.Branches on t.BranchId equals b.Id
                              join tu in _context.TicketUsers on t.Id equals tu.TicketId
                              join u in _context.Users on tu.AppUserId equals u.Id
                              join urole in _context.UserRoles on u.Id equals urole.UserId
                              join role in _context.Roles on urole.RoleId equals role.Id
                              where role.Name == "SystemAdmin"
                              join u1 in _context.Users on tu.AppUserId equals u1.Id
                              join urole1 in _context.UserRoles on u1.Id equals urole1.UserId
                              join role1 in _context.Roles on urole1.RoleId equals role1.Id
                              where role.Name == "User"
                              select new TicketVM
                              {
                                  Id = t.Id,
                                  Title = t.Title,
                                  StartDate = t.StartDate,
                                  EndDate = t.EndDate,
                                  Description = t.Description,
                                  Priority = t.Priority,
                                  Domain = d.Name,
                                  Status = s.Name,
                                  Branch = b.Name,
                                  Responsabile = u,
                                  Employee =u1

                              };*/

            IQueryable<Ticket> ticketsList = _context.Tickets.Include(t => t.Domain).Include(t => t.Status).Include(t => t.Branch).
                                            Include(t=>t.Priority).Include(t => t.TicketUsers).ThenInclude(t=>t.AppUser);


            return await ticketsList.Where(t => t.TicketUsers.Any(tu => tu.AppUser.UserName.Equals(userName))).
                        OrderBy(t=>t.Priority.Name).ThenBy(t=>t.StartDate).ToListAsync();

        }

        public async Task<Ticket> GetTicket(string Id)
        {
            
            return await _context.Tickets.Include(t => t.Domain).Include(t => t.Status).Include(t => t.Branch).
                                            Include(t => t.Priority).Include(t => t.Messages.OrderByDescending(tm=>tm.Date)).
                                            ThenInclude(tm=>tm.TicketFiles).
                                            Include(t => t.TicketUsers).ThenInclude(t => t.AppUser).
                                            Where(t=>t.Id == Id).FirstOrDefaultAsync();

            

        }

        public Task<Ticket> UpdateTicket(string Id)
        {
            throw new NotImplementedException();
        }

        public async Task<Ticket> AddTicket(TicketVM ticketVM)
        {

            Ticket ticket = new Ticket();
            _mapper.Map(ticketVM, ticket);

            await _context.AddAsync(ticket);
            var result = await _context.SaveChangesAsync() > 0;

            if(!result)
            {

                return null;
            }

            return ticket;

        }
        public async Task<TicketUser> AddTicketUser(TicketUserVM ticketUserVM)
        {

            var userDB = await _userManager.FindByNameAsync(_principal.Identity.Name);

            ticketUserVM.AppUserId = userDB.Id;
            ticketUserVM.UserRole = "User";

            TicketUser ticketUser = new TicketUser();
            _mapper.Map(ticketUserVM, ticketUser);

            await _context.AddAsync(ticketUser);

            if(await _context.SaveChangesAsync() > 0)
            {

                return ticketUser;
            }

            return null;


        }

        public async Task<IEnumerable<Domain>> GetDomainsList()
        {

            return await _context.Domains.OrderBy(d=>d.Name).ToListAsync();

        }

        public async Task<Branch> GetUserBranch()
        {

            var branchId = _context.Users.Where(u => u.UserName == _principal.Identity.Name).FirstOrDefaultAsync().Result.BranchId;

            return await _context.Branches.FirstAsync(b=>b.Id == branchId);

        }

        public Task<Branch> GetBranchByName(string branchName)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Priority>> GetTicketPriorities()
        {
            return await _context.Priorities.ToListAsync();
        }

        public Task<Domain> GetDomainByName(string domainName)
        {
            throw new NotImplementedException();
        }

        public async Task<Status> GetStatusByName(string statusName)
        {
            
            return await _context.Statuses.FirstOrDefaultAsync(s=>s.Name == statusName);

        }

        public async Task<AppUser> GetUserByUserName(string userName)
        {

            return await _userManager.FindByNameAsync(userName);

        }

       
    }
}
