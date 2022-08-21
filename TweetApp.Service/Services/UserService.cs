﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using TweetApp.Model.Dto;
using TweetApp.Repository.Entities;
using TweetApp.Repository.Interfaces;
using TweetApp.Service.Services.Interfaces;
#nullable disable

namespace TweetApp.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDetailsDto>> GetAllUsers()
        {
            var userList = await _unitOfWork.User.GetAllAsync(includeProperties: "Photos");
            List<UserDetailsDto> users = new List<UserDetailsDto>();

            foreach (var user in userList)
            {
                user.Image = user == null || user.Photos.Count == 0 ? String.Empty : user.Photos.FirstOrDefault(x => x.IsMain == true).Url;
                users.Add(_mapper.Map<UserDetailsDto>(user));
            }

            return users;
        }

        public async Task<UserDetailsDto> Register(UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);
            await _unitOfWork.User.AddAsync(user);
            await _unitOfWork.Save();
            user.Image = user.Photos == null || user.Photos.Count == 0 ? String.Empty : user.Photos.FirstOrDefault(x => x.IsMain == true).Url;
            return _mapper.Map<UserDetailsDto>(user);
        }

        public async Task<bool> IsUniqueUser(string username)
        {
            var user = await FindByUsername(username); ;

            return user != null;
        }

        public async Task<UserDetailsDto> Authenticate(string username, string password)
        {
            var user = await _unitOfWork.User.GetFirstOrDefaultAsync(x => x.Email == username && x.Password == password, includeProperties: "Photos");
            if (user != null)
                user.Image = user.Photos == null || user.Photos.Count == 0 ? String.Empty : user.Photos.FirstOrDefault(x => x.IsMain == true).Url;
            return _mapper.Map<UserDetailsDto>(user);
        }

        public async Task<UserDetailsDto> FindByUsername(string username)
        {
            var user = await _unitOfWork.User.GetFirstOrDefaultAsync(x => x.Email == username, includeProperties: "Photos");
            if (user != null)
                user.Image = user.Photos.Count == 0 ? String.Empty : user.Photos.FirstOrDefault(x => x.IsMain == true).Url;
            return _mapper.Map<UserDetailsDto>(user);
        }

        public async Task<bool> ResetPassword(string username, string password)
        {
            var user = await FindByUsername(username);

            if (user == null)
                return false;
            var userObj = _mapper.Map<User>(user);
            userObj.Password = password;

            _unitOfWork.User.Update(userObj);
            await _unitOfWork.Save();

            return true;
        }

        public async Task<IEnumerable<UserDetailsDto>> FindUsersByUsername(string username)
        {
            var users = await _unitOfWork.User.GetAllAsync(x => x.Email.ToLower().StartsWith(username.ToLower()), includeProperties: "Photos");

            var usersList = new List<UserDetailsDto>();

            foreach (var user in users)
            {
                if (user != null)
                    user.Image = user.Photos == null || user.Photos.Count == 0 ? String.Empty : user.Photos.FirstOrDefault(x => x.IsMain == true).Url;
                usersList.Add(_mapper.Map<UserDetailsDto>(user));
            }

            return usersList;
        }

    }
}
