﻿using System;
using System.Linq;
using Marvin.TestTools.Test.Model;
using NUnit.Framework;

namespace Marvin.Model.Tests
{
    [TestFixture]
    public class UnitOfWorkTests
    {
        private InMemoryUnitOfWorkFactory _unitOfWorkFactory;

        [SetUp]
        public void TestSetUp() //TODO: This is a Setup for a test not for a fixture
        {
            _unitOfWorkFactory = new InMemoryUnitOfWorkFactory(Guid.NewGuid().ToString());
            _unitOfWorkFactory.Initialize();
        }

        [Test]
        public void RepositoryByTypeGetsReturned()
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                // Arrange
                var carRepo = uow.GetRepository<ICarEntityRepository>();

                // Assert
                Assert.NotNull(carRepo);
            }
        }

        [Test]
        public void RepositoryByExplicitTypeGetsReturned()
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                // Arrange -> ISportCarRepository has an explicit implementation
                var sportCarRepository = uow.GetRepository<ISportCarRepository>();

                // Assert
                Assert.NotNull(sportCarRepository);
            }
        }

        [Test]
        public void AddingEntityToUowWithSavingSavesEntityToDb()
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                // Arrange
                var carRepo = uow.GetRepository<ICarEntityRepository>();

                // Act
                carRepo.Create();

                uow.Save();
            }

            using (var uow = _unitOfWorkFactory.Create())
            {
                var carRepo = uow.GetRepository<ICarEntityRepository>();

                var cars = carRepo.GetAll();

                // Assert
                Assert.AreEqual(1, cars.Count);
            }
        }

        [Test]
        public void AddingEntityToUowWithoutSavingDoesNotSaveEntityToDb()
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                // Arrange
                var carRepo = uow.GetRepository<ICarEntityRepository>();

                // Act
                carRepo.Create();
            }

            using (var uow = _unitOfWorkFactory.Create())
            {
                var carRepo = uow.GetRepository<ICarEntityRepository>();

                var cars = carRepo.GetAll();

                // Assert
                Assert.AreEqual(0, cars.Count);
            }
        }

        [Test]
        public void AddingComplexEntityToUowWithSavingToDbAndReadingFromDb()
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                // Arrange
                var hugePocoRepository = uow.GetRepository<IHugePocoRepository>();

                // Act
                hugePocoRepository.Create(1.0, 2, 3.5, 3, 4.8, 5, 6.7, 8, 9.2, 10);

                uow.Save();
            }

            using (var uow = _unitOfWorkFactory.Create())
            {
                var hugePocoRepository = uow.GetRepository<IHugePocoRepository>();

                var hugePocs = hugePocoRepository.GetAll();

                // Assert
                Assert.AreEqual(1, hugePocs.Count);

                var hugePoco = hugePocs.First();
                Assert.AreEqual(1.0, hugePoco.Float1);
                Assert.AreEqual(2, hugePoco.Number1);
            }
        }
    }
}