using antigal.server.Models;
using antigal.server.Validaciones;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace Antigal.Server.Tests;

[TestClass]
public class ValidatorTests
{
    [TestMethod]
    public void Categoria_ValidData_Passes()
    {
        var validator = new ValidacionCategoria();
        var categoria = new Categoria
        {
            nombre = "Frutos secos",
            descripcion = "Productos naturales"
        };

        var result = validator.Validate(categoria);

        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public void Categoria_ShortName_Fails()
    {
        var validator = new ValidacionCategoria();
        var categoria = new Categoria { nombre = "AB" };

        var result = validator.Validate(categoria);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(error => error.PropertyName == nameof(Categoria.nombre)));
    }

    [TestMethod]
    public void Categoria_LongDescription_Fails()
    {
        var validator = new ValidacionCategoria();
        var categoria = new Categoria
        {
            nombre = "Cereales",
            descripcion = new string('x', 501)
        };

        var result = validator.Validate(categoria);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(error => error.PropertyName == nameof(Categoria.descripcion)));
    }

    [TestMethod]
    public void Producto_ValidData_Passes()
    {
        var validator = new ValidacionProducto();
        var producto = new Producto
        {
            nombre = "Avena",
            marca = "Antigal",
            precio = 1200m,
            stock = 4
        };

        var result = validator.Validate(producto);

        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public void Producto_EmptyBrand_Fails()
    {
        var validator = new ValidacionProducto();
        var producto = new Producto
        {
            nombre = "Avena",
            marca = "",
            precio = 1200m,
            stock = 4
        };

        var result = validator.Validate(producto);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(error => error.PropertyName == nameof(Producto.marca)));
    }

    [TestMethod]
    public void Producto_NonPositivePrice_Fails()
    {
        var validator = new ValidacionProducto();
        var producto = new Producto
        {
            nombre = "Avena",
            marca = "Antigal",
            precio = 0m,
            stock = 4
        };

        var result = validator.Validate(producto);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(error => error.PropertyName == nameof(Producto.precio)));
    }

    [TestMethod]
    public void Producto_NegativeStock_Fails()
    {
        var validator = new ValidacionProducto();
        var producto = new Producto
        {
            nombre = "Avena",
            marca = "Antigal",
            precio = 1200m,
            stock = -1
        };

        var result = validator.Validate(producto);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(error => error.PropertyName == nameof(Producto.stock)));
    }

    [TestMethod]
    public void Producto_VerificarDisponible_ReflectsCurrentStock()
    {
        var producto = new Producto
        {
            nombre = "Avena",
            marca = "Antigal",
            precio = 1200m,
            stock = 3
        };

        Assert.AreEqual(1, producto.verificarDisponible());

        producto.stock = 0;
        Assert.AreEqual(0, producto.verificarDisponible());
    }
    [TestMethod]
    public void ValidationServices_SharpGripAutoValidation_RegistersWithMvc()
    {
        var services = new ServiceCollection();
        services.AddControllers();
        var countBeforeAutoValidation = services.Count;

        services.AddFluentValidationAutoValidation();

        Assert.IsTrue(services.Count > countBeforeAutoValidation);
    }

    [TestMethod]
    public void ValidationServices_AssemblyRegistration_ResolvesMaintainedValidators()
    {
        var services = new ServiceCollection();
        services.AddValidatorsFromAssemblyContaining<ValidacionCategoria>();

        using var provider = services.BuildServiceProvider();

        Assert.IsInstanceOfType(provider.GetRequiredService<IValidator<Categoria>>(), typeof(ValidacionCategoria));
        Assert.IsInstanceOfType(provider.GetRequiredService<IValidator<Producto>>(), typeof(ValidacionProducto));
    }

}
