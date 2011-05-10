Occasionally you want a complex format strings. Not bigger enough or enough of them to use a [full blown template system](http://razorengine.codeplex.com/). Rather than

    var fields = new { name = "Bob", zipCode = "AB1234", phone =   "01234 567890" };
    var address = String.Format("{0} : {1}", fields.name, fields.zipCode);
    var phone = String.Format("{0} : {1}", fields.name, fields.phone);

it would be nice to write

    var fields = new { name = "Bob", zipCode = "AB1234", phone = "01234 567890" };
    var address = String.Format("{name} : {zipCode}", fields);
    var phone = String.Format("{name} : {phone}", fields);

StringFill tries to provide an extension method `Fill`  for  `System.String` and `AppendFill` for `System.Text.StringBuilder` to provide this:

    var sb = new StringBuilder();
    sb.AppendFill("Hello {name}", new {name = "World"});

or

    string actual = StringFill.Fill("exec SomeProc({arg1}, {arg2});",
                                    new { arg1 = "Test", arg2 = "Example" });

## Questions

**Can I use it with Dictionary&lt;string, object&gt;?** Yes, if you pass in a 
generic dictionary of string to anything it will perform the look up based on
the keys of the dictionary rather than the properties.