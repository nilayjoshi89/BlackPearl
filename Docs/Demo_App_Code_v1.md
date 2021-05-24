## Explaining Demo Application Code

Defining Person:

```csharp
public class Person
{
    public string Name { get; set; }
    public string Company { get; internal set; }
    public string City { get; internal set; }
    public string Zip { get; internal set; }
    public string Info
    {
       get => $"{Name} - {Company}({Zip})";
    }
}
```

### 1) Simple Scenario (most common)
We're setting `DisplayMemberPath` to 'Name' value to display `Name` of `Person` in control. We only need to define `ItemSource` and `SelectedItems` bindings. That's it!

![](Docs//1_Control.png)

#### .XAML code:

```xml
<controls:MultiSelectCombobox ItemSource="{Binding Source, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
                              SelectedItems="{Binding SelectedItems, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
                              DisplayMemberPath="Name"
                              ItemSeparator=";"/>
```

### 2) Advance Scenario
If we want filtering on more than one property or need different search/filter strategy. And/or also want to support creation of new Person from MultiSelectCombobox itself.

![](Docs/2_Control.png)

#### .XAML code:

```xml
<controls:MultiSelectCombobox ItemSource="{Binding Source, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
                              SelectedItems="{Binding SelectedItems2, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
                              DisplayMemberPath="Info"
                              ItemSeparator=";"
                              LookUpContract="{Binding AdvanceLookUpContract}"/>
```


In Xaml, we have set DisplayMemberPath to Info property. Info is set to return Name, Company and ZipCode.

**`AdvanceLookUpContract.cs`:**
In this implementation, we have modified search to respect 3 properties on Person. If any of these 3 properties contain search string, item will be shown in Suggestion drop-down. Item is selected from ItemSource based on Name property. We have also set SupportsNewObjectCreation to true which means we can create new Person object using control. CreateObject is written to parse string in format `{Name},{Company},{Zip}`. By inputting string in this format ending with ItemSeparator, it will try to create an object out of inputted string. If it fails to create, it will remove User inputted string from UI. If it succeeds to create object, it will add newly created object to UI and SelectedItems after removing User entered text from UI.

**[Please note that following implementation is just for demonstration purpose of LookUpContract functionality. This implementation is not efficient and has lot of scope for improvements.]**

```csharp
public class AdvanceLookUpContract : ILookUpContract
{
    public bool SupportsNewObjectCreation => true;

    public object CreateObject(object sender, string searchString)
    {
        if (searchString?.Count(c => c == ',') != 2)
        {
            return null;
        }

        int firstIndex = searchString.IndexOf(',');
        int lastIndex = searchString.LastIndexOf(',');

        return new Person()
        {
            Name = searchString.Substring(0, firstIndex),
            Company = searchString.Substring(firstIndex + 1, lastIndex - firstIndex - 1),
            Zip = searchString.Length >= lastIndex ? searchString.Substring(lastIndex + 1) : string.Empty
        };
    }

    public bool IsItemEqualToString(object sender, object item, string seachString)
    {
        if (!(item is Person std))
        {
            return false;
        }

        return string.Compare(seachString, std.Name, System.StringComparison.InvariantCultureIgnoreCase) == 0;
    }

    public bool IsItemMatchingSearchString(object sender, object item, string searchString)
    {
        if (!(item is Person person))
        {
            return false;
        }

        if (string.IsNullOrEmpty(searchString))
        {
            return true;
        }

        return person.Name?.ToLower()?.Contains(searchString?.ToLower()) == true
            || person.Company.ToString().ToLower()?.Contains(searchString?.ToLower()) == true
            || person.Zip?.ToLower()?.Contains(searchString?.ToLower()) == true;
    }
}
```
