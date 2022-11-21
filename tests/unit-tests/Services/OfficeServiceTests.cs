using shopping_bag.Config;
using shopping_bag.DTOs.Office;
using shopping_bag.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shopping_bag_unit_tests.Services
{
    public class OfficeServiceTests : BaseServiceTest
    {
        private readonly AppDbContext _context;
        private readonly IOfficeService _sut;

        public OfficeServiceTests() : base()
        {
            _context = GetDatabase();
            _sut = new OfficeService(_context, UnitTestHelper.GetMapper());
        }

        #region AddOffice Tests
        [Fact]
        public async Task AddOffice_ValidOffice_OfficeAdded()
        {
            var newOffice = new AddOfficeDto() { Name = "Turku" };
            var response = _sut.AddOffice(newOffice);
            Assert.NotNull(response.Result);
        }

        [Fact]
        public async Task AddOffice_InValidOffice_OfficeNotAdded()
        {
            var newOffice = new AddOfficeDto() { Name = "Tampere" };
            var response = _sut.AddOffice(newOffice);
            Assert.False(response.Result.IsSuccess);
            Assert.Equal("Office with that name already exists", response.Result.Error);
        }

        [Fact]
        public async Task AddOffice_RemovedOfficeWithSameName_OfficeUnRemoved()
        {
            var newOffice = new AddOfficeDto() { Name = "Karibia" };
            var response = _sut.AddOffice(newOffice);
            Assert.True(response.Result.IsSuccess);
        }

        #endregion

        #region ModifyOffice Tests

        [Fact]
        public async Task ModifyOffice_ValidOffice_OfficeModified()
        {
            var modifyOffice = new AddOfficeDto() { Name = "Turku" };
            var response = _sut.ModifyOffice(modifyOffice, ListOffice.Id);
            Assert.True(response.Result.IsSuccess);
        }

        [Fact]
        public async Task ModifyOffice_OfficeWithSameName_OfficeNotModified()
        {
            var modifyOffice = new AddOfficeDto() { Name = "Helsinki" };
            var response = _sut.ModifyOffice(modifyOffice, TestOffice.Id);
            Assert.False(response.Result.IsSuccess);
            Assert.Equal("Office with that name already exists", response.Result.Error);
        }

        [Fact]
        public async Task ModifyOffice_RemovedOffice_OfficeNotModified()
        {
            var modifyOffice = new AddOfficeDto() { Name = "Helsinki" };
            var response = _sut.ModifyOffice(modifyOffice, RemovedOffice.Id);
            Assert.False(response.Result.IsSuccess);
            Assert.Equal("Invalid office id", response.Result.Error);
        }

        #endregion

        #region RemoveOffice Tests

        [Fact]
        public async Task RemoveOffice_ValidOffice_OfficeRemoved()
        {
            var response = _sut.RemoveOffice(TestOffice.Id);
            Assert.True(response.Result.IsSuccess);
        }

        [Fact]
        public async Task RemoveOffice_AlreadyRemoved_OfficeNotRemoved()
        {
            var response = _sut.RemoveOffice(RemovedOffice.Id);
            Assert.False(response.Result.IsSuccess);
            Assert.Equal("Invalid office id", response.Result.Error);
        }

        #endregion

        #region GetOffices Tests
        [Fact]
        public async Task GetOffices_ThreeOfficesAdded_OnlyNonRemovedReturned()
        {
            var offices = await _sut.GetOffices();
            Assert.True(offices.IsSuccess);
            // Three offices exist, but the one is removed
            Assert.Equal(2, offices.Data.Count());
        }
        #endregion
    }
}
