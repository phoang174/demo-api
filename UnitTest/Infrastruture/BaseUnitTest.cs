using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.IRepository;
using Moq;

namespace UnitTest.Infrastruture
{

    namespace UnitTest
    {
        public abstract class BaseUnitTest
        {
            protected readonly Mock<IUserRepository> _userRepository;
            protected readonly Mock<IBlackListRepository> _blackListRepository;
            protected readonly Mock<IRoleRepository> _roleRepository;


            protected BaseUnitTest()
            {
                _userRepository = new Mock<IUserRepository>();
                _blackListRepository = new Mock<IBlackListRepository>();
                _roleRepository = new Mock<IRoleRepository>();
            }
        }
    }

}
