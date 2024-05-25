using Repository.GenericRepository;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.UnitOfwork
{
    public class UnitOfwork : IDisposable, IUnitOfwork
    {

        private IGenericRepository<Book> bookRepo;
        private IGenericRepository<Category> categoryRepo;
        private IGenericRepository<User> userRepo;
        private IGenericRepository<Order> orderRepo;
        private IGenericRepository<OrderDetail> orderDetailRepo;
        private IGenericRepository<Role> roleRepo;
        private IGenericRepository<Token> tokenRepo;
        private IGenericRepository<Cart> cartRepo;
        private readonly BStoreDBContext context;
        private bool dispose = false;

        public UnitOfwork(BStoreDBContext context)
        {
            this.context = context;
        }

        public IGenericRepository<Book> BookRepo
        {
            get
            {
                if (bookRepo == null)
                {
                    bookRepo = new GenericRepository<Book>(context);
                }
                return bookRepo;

            }
            set => throw new NotImplementedException();
        }
        public IGenericRepository<Category> CategoryRepo
        {
            get
            {
                if (categoryRepo == null)
                {
                    categoryRepo = new GenericRepository<Category>(context);
                }
                return categoryRepo;

            }
            set => throw new NotImplementedException();
        }
        public IGenericRepository<User> UserRepo
        {
            get
            {
                if (userRepo == null)
                {
                    userRepo = new GenericRepository<User>(context);
                }
                return userRepo;

            }
            set => throw new NotImplementedException();
        }
        public IGenericRepository<Order> OrderRepo
        {
            get
            {
                if (orderRepo == null)
                {
                    orderRepo = new GenericRepository<Order>(context);
                }
                return orderRepo;

            }
            set => throw new NotImplementedException();
        }
        public IGenericRepository<OrderDetail> OrderDetailRepo
        {
            get
            {
                if (orderDetailRepo == null)
                {
                    orderDetailRepo = new GenericRepository<OrderDetail>(context);
                }
                return orderDetailRepo;

            }
            set => throw new NotImplementedException();
        }

        public IGenericRepository<Role> RoleRepo
        {
            get
            {
                if (roleRepo == null)
                {
                    roleRepo = new GenericRepository<Role>(context);
                }
                return roleRepo;

            }
            set => throw new NotImplementedException();
        }

        public IGenericRepository<Token> TokenRepo
        {
            get
            {
                if (tokenRepo == null)
                {
                    tokenRepo = new GenericRepository<Token>(context);
                }
                return tokenRepo;

            }
            set => throw new NotImplementedException();
        }

        public IGenericRepository<Cart> CartRepo
        {
            get
            {
                if (cartRepo == null)
                {
                    cartRepo = new GenericRepository<Cart>(context);
                }
                return cartRepo;

            }
            set => throw new NotImplementedException();
        }

        protected virtual void Dispose(bool dispose)
        {
            if (!dispose)
            {
                if (dispose)
                {
                    context.Dispose();

                }
                dispose = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
