using System.Linq;
using System.Collections.Generic;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
namespace API.Data
{
    public class PhotoRepository: IPhotoRepository
    {
        private readonly DataContext _context;

        private readonly IMapper _mapper;
        public PhotoRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<Photo> GetPhotoById(int id)
        {
             return await _context.Photos.FindAsync(id);
        }

        public void RemovePhoto(Photo photo)
        {
            _context.Photos.Remove(photo);
        }

        public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
        {
            var UnapprovedPhotos = await _context.Photos
            .Where(p => p.IsApproved == false)
            .ProjectTo<PhotoForApprovalDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

            return UnapprovedPhotos;

        }
    }
}