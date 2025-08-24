namespace RestApiProject.Profiles
{
    using AutoMapper;
    using RestApiProject.Models;
    using RestApiProject.DTOs;
    public class BookProfile: Profile
    {

        public BookProfile() 
        {
            CreateMap<Book, BookDTO>();
            CreateMap<BookDTO, Book>();

            CreateMap<BookCreationDto, Book>();
            CreateMap<Book, BookCreationDto>();
        }

    }
}
