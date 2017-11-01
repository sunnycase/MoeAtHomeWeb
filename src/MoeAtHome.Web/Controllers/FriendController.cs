using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MoeAtHome.Web.Models;
using MoeAtHome.Web.Repository;

namespace MoeAtHome.Web.Controllers
{
    [Route("api/[controller]")]
    public class FriendController : Controller
    {
        private readonly IFriendRepository _friendRepo;
        private readonly IMapper _mapper;

        public FriendController(IFriendRepository friendRepository, IMapper mapper)
        {
            _friendRepo = friendRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(_mapper.Map<IReadOnlyList<FriendViewModel>>(await _friendRepo.GetAllAsync()));
        }
    }
}
