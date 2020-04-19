# About repository
This repo was created to put different basic WPF Custom-control/User-controls under same roof. These controls solve basic and frequently faced issue by developers. As of now, this repo contains one user-control **MultiSelectCombobox**. In future, I'm planning to add more.
***
# MultiSelectCombobox
WPF has ListBox control which lets user select more than one item. However, ListBox control UI doesn't have in-built support for searching/filtering. Developers have to do work around to provision one. Moreover, lot of mouse interaction is required. Yes, you may be able to do all completely using keyboard. On the other hand, Combobox has a very good UI which supports Searching and filtering. However, it doesn't support multiple selection.

What if we can combine behavior of ListBox and goodness of Combobox UI? MultiSelectCombobox exactly does the same thing. It provides functionality of searching/filtering with multiple selection. MultiSelectCombobox tries to mimic UI behavior of ComboBox.
***
## Feature
* In built support for searching and filtering
* Extensible to support custom searching and filtering for Complex type
* Ability to add item which is not part of source collection (through LookUpContract for complex types)
* Easy to use!

![](https://github.com/nilayjoshi89/BlackPearl/blob/master/Docs/0_App.png)
***



## Design
MultiSelectCombobox is composed of RichTextBox, Popup and ListBox. Text entered in RichTextBox is monitored and manipulated. If it finds suitable item from source collection, it will replace entered text selected item. Selected item is shown as TextBlock - Inline UI element. On key press, popup box will show up and display items matching search criteria. If there is no item in collection matching search criteria, it won't show up.

### Dependency Properties
Control is designed to expose minimal properties which are required to make it work. 

**1. ItemSource (IEnumerable)** - Source collection should be bound to this property. It support collection of as simple type as string to complex type/entities.

**2. SelectedItems (IList)** - This property will provide collection of items selected by user.

**3. ItemSeparator (char)** - default value is ';'. In control, items are separated with ItemSeparator char. This is important if items contain spaces. Separator should be chosen carefully. Moreover, to indicate end of item while entering or forcing control to create new item based on current entered text, this character it used. Also, if user enters text which does not match any item provided in collection or LookUpContract does not support creation of object from given text, user entered text will be removed from control UI. Support for creation of new item is discussed later in this document.

**4. DisplayMemberPath (string)** - If ItemSource collection is of complex type, developer may need to override ToString() method of type or else can define DisplayMemberPath property. Default value is string.Empty.

**5. SelectedItemTextBlockStyle (Style)** - SelectedItems are shown as Inline element of TextBlocks in RichTextBox document. If you want to change default style of selected item TextBlocks, you can assign new Style to this property.

**6. LookUpContract (ILookUpContract)** - This property is used to customize searching/filtering behavior of the control. Control provides default implementation which works for most users. However, in case of Complex type and/or custom behavior, user can provide implementation and change control behavior.

### Explaining LookUpContract (ILookUpContract)

Default search/filtering work on string.StartsWith & string.Equals respectively. For any given item, if DisplayMemberPath is not set, item.ToString() value is sent to filtering mechanism. If DisplayMemberPath is provided, path value is fetched through item reflection and sent to filter mechanism. This works for most of the user.

However, if user needs customize these setting/filtering mechanism, he/she can provide implementation of this interface and bind to LookUpContract property. Control will respect newly bound implementation.

**ILookUpContract.cs**

    public interface ILookUpContract
	{
		// Whether contract supports creation of new object from user entered text
		bool SupportsNewObjectCreation { get; }
		
		// Method to check if item matches searchString
		bool IsItemMatchingSearchString(object sender, object item, string searchString);
				
		// Checks if item matches searchString or not
		bool IsItemEqualToString(object sender, object item, string seachString);
				
		// Creates object from provided string
		// This method need to be implemented only when SupportsNewObjectCreation is set to true
		object CreateObject(object sender, string searchString);
	}

**IsItemMatchingSearchString** - This function is called to filter suggestion item in drop-down list. User entered text is passed as parameter to this function. Return true if item should be displayed in suggestion drop-down for given text. Otherwise return false.

**IsItemEqualToString** - This function is used to find item from collection based on user entered text.

**CreateObject** - This function should only be implemented if SupportsNewObjectCreation is set to true. This function is called to create object based on provided text. Also, we can create complex object by entering comma separated value in control ending with ItemSeparator. 

For example, if we have assigned collection of Complex type Student having two property Name and Age. ItemSeparator is set to ';'. Entered text can be - StudentNameHere, ThisIsAgeValue. Function will receive this text as input. Function should separate string by comma and then set first value to Name and second value to Age property and return Student object. This is one way of implementation. You can define parsing mechanism the way user want.

**SupportsNewObjectCreation** - If this property is set to false, control will not allow user to select item other than provided collection (ItemSource). If this property is set to true, control will allow creation of new object. This is useful when control should let user add new object. Also eliminates need to create separate TextBox(es) and button to add new item in existing SelectedItems/ItemSource.

Complete example is provided in Demo application. Sample implementation - [AdvanceLookUpContract](https://github.com/nilayjoshi89/BlackPearl/blob/master/BlackPearl.Controls.Demo/AdvanceLookUpContract.cs)

**[DefaultLookUpContract](https://github.com/nilayjoshi89/BlackPearl/blob/master/BlackPearl.Controls.Library/Control/ILookUpContract.cs)** - If no new implementation is provided to control, this DefaultLookUpContract implementation is used. This contract uses string.StartsWith for searching and string.Equals for comparison. Both comparison is invariant of culture and case.

## Usage

Definition of Person

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

### 1) Simple Scenario 
We're setting DisplayMemberPath to Name to show Name of Person in control. We want to perform simple filtering on Name property. We only need to provide ItemSource and SelectedItems collection to control to function.

![](https://github.com/nilayjoshi89/BlackPearl/blob/master/Docs/1_Control.png)

**.XAML code:**

	<controls:MultiSelectCombobox ItemSource="{Binding Source, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
                                              SelectedItems="{Binding SelectedItems, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
                                              DisplayMemberPath="Name"
                                              ItemSeparator=";"/>

### 2) Complex Scenario
If we want filtering on more than one property or need different search/filter strategy. And/or also want to support creation of new Person from UI itself.

![](https://github.com/nilayjoshi89/BlackPearl/blob/master/Docs/2_Control.png)

**.XAML code:**

	<controls:MultiSelectCombobox ItemSource="{Binding Source, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
                                              SelectedItems="{Binding SelectedItems2, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
                                              DisplayMemberPath="Info"
                                              ItemSeparator=";"
                                              LookUpContract="{Binding AdvanceLookUpContract}"/>

In Xaml, we have set DisplayMemberPath to Info property. Info is set to return Name, Company and ZipCode.

**AdvanceLookUpContract.cs:**
In this implementation, we have modified search to respect 3 properties on Person. If any of these 3 properties contain search string, item will be shown in Suggestion drop-down. Item is selected from ItemSource based on Name property. We have also set SupportsNewObjectCreation to true which means we can create new Person object using control. CreateObject is written to parse string in format `{Name},{Company},{Zip}`. By inputting string in this format ending with ItemSeparator, it will try to create an object out of inputted string. If it fails to create, it will remove User inputted string from UI. If it succeeds to create object, it will add newly created object to UI and SelectedItems after removing User entered text from UI.

	public class AdvanceLookUpContract : MultiSelectCombobox.ILookUpContract
    {
        public bool SupportsNewObjectCreation => true;

        public object CreateObject(object sender, string searchString)
        {
            if (searchString?.Count(c => c == ',') != 2)
            {
                return null;
            }

            var firstIndex = searchString.IndexOf(',');
            var lastIndex = searchString.LastIndexOf(',');

            return new Person()
            {
                Name = searchString.Substring(0, firstIndex),
                Company = searchString.Substring(firstIndex + 1, lastIndex - firstIndex - 1),
                Zip = searchString.Length >=lastIndex ? searchString.Substring(lastIndex + 1) : string.Empty
            };
        }

        public bool IsItemEqualToString(object sender, object item, string seachString)
        {
            if (!(item is Person std))
                return false;

            return string.Compare(seachString, std.Name, System.StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        public bool IsItemMatchingSearchString(object sender, object item, string searchString)
        {
            if (!(item is Person person))
                return false;

            if (string.IsNullOrEmpty(searchString))
                return true;

            return person.Name?.ToLower()?.Contains(searchString?.ToLower()) == true
                || person.Company.ToString().ToLower()?.Contains(searchString?.ToLower()) == true
                || person.Zip?.ToLower()?.Contains(searchString?.ToLower()) == true;
        }
    }

For complete solution, please refer to Demo application.
