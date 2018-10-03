# SFA.DAS.Validation

Automatic model validation for MVC applications that are using `System.ComponentModel.DataAnnotations` and the OMIOMO (one model in, one model out) & PRG (Post/Redirect/Get) patterns.

## Configuration

### MVC

```c#
filters.AddValidationFilter();
```

### MVC Core

Coming soon...

## Example

```c#
public class CreateAccountViewModel
{
    [Required]
    public CreateAccountCommand CreateAccountCommand { get; set; }
}
```

```c#
public class CreateAccountCommand : IRequest
{
    [Required(ErrorMessage = "The 'Name' field is required")]
    public string Name { get; set; }
}
```

```c#
[RoutePrefix("accounts")]
public class AccountsController : Controller
{
    private readonly IMediator _mediator;

    public AccountsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Before this action method is called if there are any errors in temp data then they are added to the controller's model state
    [Route]
    public ActionResult Create()
    {
        return View(new CreateAccountViewModel());
    }

    // Before this action method is called if there are any errors in the controller's model state then they are added to temp data and a redirect result to GET /accounts is returned
    [HttpPost]
    [Route]
    public async Task<ActionResult> Create(CreateAccountViewModel model)
    {
        await _mediator.Send(model.CreateAccountCommand);

        return RedirectToAction("Index", "Home");
    }
}
```

```
<h1>Create account</h1>
<p>Complete and submit the following form to create an account.</p>

@using (Html.BeginForm())
{
    <div>
        <label for="@Html.IdFor(m => m.CreateAccountCommand.Name)">Name</label>
        @Html.ValidationMessageFor(m => m.CreateAccountCommand.Name) @*Rendered only when there's a model state error for Name*@
        @Html.TextBoxFor(m => m.CreateAccountCommand.Name)
    </div>

    <button type="submit">Submit</button>
}
```
