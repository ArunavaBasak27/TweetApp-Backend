using AutoMapper;
using Microsoft.AspNetCore.Http;
using TweetApp.Model.Dto;
using TweetApp.Repository.Entities;
using TweetApp.Repository.Interfaces;
using TweetApp.Service.Services.Interfaces;
#nullable disable

namespace TweetApp.Service.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPhotoAccessor _photoAccessor;
        public PhotoService(IUnitOfWork unitOfWork, IPhotoAccessor photoAccessor,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _photoAccessor = photoAccessor;
        }

        public async Task<UserDetailsDto> AddPhoto(string username, IFormFile file)
        {
            var user = await _unitOfWork
                .User.GetFirstOrDefaultAsync(x => x.Email == username, includeProperties: "Photos");
            if (user == null) return null;

            var photoUploadResult = await _photoAccessor.AddPhoto(file);

            var photo = new Photo
            {
                Url = photoUploadResult.Url,
                PublicId = photoUploadResult.PublicId
            };

            if (!user.Photos.Any(x => x.IsMain))
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            await _unitOfWork.Save();

            var response=_mapper.Map<UserDetailsDto>(user);

            if(!response.Photos.Any(x=>x.IsMain))
            {
                response.Image = photo.Url;
            }

            return response;
        }
    }
}
