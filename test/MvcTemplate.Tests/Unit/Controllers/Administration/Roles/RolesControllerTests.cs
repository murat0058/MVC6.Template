﻿using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Routing;
using MvcTemplate.Controllers.Administration;
using MvcTemplate.Objects;
using MvcTemplate.Services;
using MvcTemplate.Validators;
using NSubstitute;
using System;
using System.Linq;
using Xunit;

namespace MvcTemplate.Tests.Unit.Controllers.Administration
{
    public class RolesControllerTests : AControllerTests
    {
        private RolesController controller;
        private IRoleValidator validator;
        private IRoleService service;
        private RoleView role;

        public RolesControllerTests()
        {
            validator = Substitute.For<IRoleValidator>();
            service = Substitute.For<IRoleService>();
            role = new RoleView();

            controller = Substitute.ForPartsOf<RolesController>(validator, service);
            controller.ActionContext.RouteData = new RouteData();
        }

        #region Method: Index()

        [Fact]
        public void Index_GetsRoleViews()
        {
            service.GetViews().Returns(new RoleView[0].AsQueryable());

            Object actual = controller.Index().ViewData.Model;
            Object expected = service.GetViews();

            Assert.Same(expected, actual);
        }

        #endregion

        #region Method: Create()

        [Fact]
        public void Create_ReturnsNewRoleView()
        {
            RoleView actual = controller.Create().ViewData.Model as RoleView;

            Assert.NotNull(actual.PrivilegesTree);
            Assert.Null(actual.Title);
        }

        [Fact]
        public void Create_SeedsPrivilegesTree()
        {
            RoleView view = controller.Create().ViewData.Model as RoleView;

            service.Received().SeedPrivilegesTree(view);
        }

        #endregion

        #region Method: Create(RoleView role)

        [Fact]
        public void Create_ProtectsFromOverpostingId()
        {
            ProtectsFromOverpostingId(controller, "Create");
        }

        [Fact]
        public void Create_SeedsPrivilegesTreeIfCanNotCreate()
        {
            validator.CanCreate(role).Returns(false);

            controller.Create(role);

            service.Received().SeedPrivilegesTree(role);
        }

        [Fact]
        public void Create_ReturnsSameModelIfCanNotCreate()
        {
            validator.CanCreate(role).Returns(false);

            Object actual = (controller.Create(role) as ViewResult).ViewData.Model;
            Object expected = role;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Create_CreatesRole()
        {
            validator.CanCreate(role).Returns(true);

            controller.Create(role);

            service.Received().Create(role);
        }

        [Fact]
        public void Create_AfterCreateRedirectsToIndex()
        {
            validator.CanCreate(role).Returns(true);
            controller.When(sub => sub.RedirectIfAuthorized("Index")).DoNotCallBase();
            controller.RedirectIfAuthorized("Index").Returns(new RedirectToActionResult(null, null, null));

            ActionResult expected = controller.RedirectIfAuthorized("Index");
            ActionResult actual = controller.Create(role);

            Assert.Same(expected, actual);
        }

        #endregion

        #region Method: Details(String id)

        [Fact]
        public void Details_ReturnsNotEmptyView()
        {
            service.GetView(role.Id).Returns(role);
            controller.When(sub => sub.NotEmptyView(role)).DoNotCallBase();
            controller.NotEmptyView(role).Returns(new RedirectToActionResult(null, null, null));

            Object expected = controller.NotEmptyView(role);
            Object actual = controller.Details(role.Id);

            Assert.Same(expected, actual);
        }

        #endregion

        #region Method: Edit(String id)

        [Fact]
        public void Edit_ReturnsNotEmptyView()
        {
            service.GetView(role.Id).Returns(role);
            controller.When(sub => sub.NotEmptyView(role)).DoNotCallBase();
            controller.NotEmptyView(role).Returns(new RedirectToActionResult(null, null, null));

            Object expected = controller.NotEmptyView(role);
            Object actual = controller.Edit(role.Id);

            Assert.Same(expected, actual);
        }

        #endregion

        #region Method: Edit(RoleView role)

        [Fact]
        public void Edit_SeedsPrivilegesTreeIfCanNotEdit()
        {
            validator.CanEdit(role).Returns(false);

            controller.Edit(role);

            service.Received().SeedPrivilegesTree(role);
        }

        [Fact]
        public void Edit_ReturnsSameModelIfCanNotEdit()
        {
            validator.CanEdit(role).Returns(false);

            Object actual = (controller.Edit(role) as ViewResult).ViewData.Model;
            Object expected = role;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Edit_EditsRole()
        {
            validator.CanEdit(role).Returns(true);

            controller.Edit(role);

            service.Received().Edit(role);
        }

        [Fact]
        public void Edit_AfterEditRedirectsToIndex()
        {
            validator.CanEdit(role).Returns(true);
            controller.When(sub => sub.RedirectIfAuthorized("Index")).DoNotCallBase();
            controller.RedirectIfAuthorized("Index").Returns(new RedirectToActionResult(null, null, null));

            ActionResult expected = controller.RedirectIfAuthorized("Index");
            ActionResult actual = controller.Edit(role);

            Assert.Same(expected, actual);
        }

        #endregion

        #region Method: Delete(String id)

        [Fact]
        public void Delete_OnModelNotFoundRedirectsToNotFound()
        {
            service.GetView(role.Id).Returns(role);
            controller.When(sub => sub.NotEmptyView(role)).DoNotCallBase();
            controller.NotEmptyView(role).Returns(new RedirectToActionResult(null, null, null));

            Object expected = controller.NotEmptyView(role);
            Object actual = controller.Delete(role.Id);

            Assert.Same(expected, actual);
        }

        #endregion

        #region Method: DeleteConfirmed(String id)

        [Fact]
        public void DeleteConfirmed_DeletesRoleView()
        {
            controller.DeleteConfirmed(role.Id);

            service.Received().Delete(role.Id);
        }

        [Fact]
        public void Delete_AfterDeleteRedirectsToIndex()
        {
            controller.When(sub => sub.RedirectIfAuthorized("Index")).DoNotCallBase();
            controller.RedirectIfAuthorized("Index").Returns(new RedirectToActionResult(null, null, null));

            ActionResult expected = controller.RedirectIfAuthorized("Index");
            ActionResult actual = controller.DeleteConfirmed(role.Id);

            Assert.Same(expected, actual);
        }

        #endregion
    }
}