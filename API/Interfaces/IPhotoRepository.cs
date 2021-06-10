using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using System.Collections.Generic;

namespace API.Interfaces
{
    public interface IPhotoRepository
    {
        Task<Photo> GetPhotoById(int id);

        Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos();

        void RemovePhoto(Photo photo);

    }
}