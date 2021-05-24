# MultiSelectCombobox
## Content:
* [Overview](#overview)
* [Design](#design-overview)
* [Dependency Properties](#dependency-properties)
* [Explaining Demo Code](Demo_App_Code_v1.md)

## Overview
WPF has ListBox control which lets user select more than one item. However, ListBox control UI doesn't have in-built support for searching/filtering. Developers have to do work around to provision one. Moreover, lot of mouse interaction is required. Yes, you may be able to do all completely using keyboard. But, not most efficient way of doing. On the other hand, Combobox has a very good UI which supports searching and filtering. However, it doesn't support multiple selection.

What if we can combine behavior of ListBox and goodness of Combobox UI? MultiSelectCombobox exactly does the same thing. It provides functionality of searching/filtering with multiple selection. MultiSelectCombobox tries to mimic UI behavior of ComboBox.

![](App2.0_2.gif)

***

## Design Overview
MultiSelectCombobox is composed of RichTextBox, Popup and ListBox. Text entered in RichTextBox is monitored and manipulated. On key press, popup box will show up and display items from source collection matching search criteria. If there is no matching item in collection, it won't show up. If it finds suitable item from source collection, it will replace entered text with source collection item. Selected item is shown as TextBlock - Inline UI element.

Individual control placement and behavior can be changed with Control template. Template parts are defined as following:
```csharp
    [TemplatePart(Name = "rtxt", Type = typeof(RichTextBox))]
    [TemplatePart(Name = "popup", Type = typeof(Popup))]
    [TemplatePart(Name = "lstSuggestion", Type = typeof(ListBox))]
    public sealed partial class MultiSelectCombobox : Control
    {
```

### Dependency Properties
Control is designed to expose minimal properties which are required to make it work. 

**1. `ItemSource (IEnumerable)`** - Source collection should be bound to this property. It support collection of as simple type as string to complex type/entities.

**2. `SelectedItems (IList)`** - This property will provide collection of items selected by user.

**3. `ItemSeparator (char)`** - default value is ';'. In control, items are separated with ItemSeparator char. This is important if items contain spaces. Separator should be chosen carefully. Moreover, to indicate end of item while entering or forcing control to create new item based on current entered text, this character it used. Also, if user enters text which does not match any item provided in collection or LookUpContract does not support creation of object from given text, user entered text will be removed from control UI. Support for creation of new item is discussed later in this document.

**4. `DisplayMemberPath (string)`** - If ItemSource collection is of complex type, developer may need to override ToString() method of type or else can define DisplayMemberPath property. Default value is string.Empty.

**5. `LookUpContract (ILookUpContract)`** - This property is used to customize searching/filtering behavior of the control. Control provides default implementation which works for most users. However, in case of Complex type and/or custom searching/filtering behavior, user can provide implementation and change control behavior.

#### Explaining LookUpContract (`ILookUpContract`) for advance scenarios

Default search/filtering work on string.StartsWith & string.Equals respectively. For any given item, if DisplayMemberPath is not set, item.ToString() value is sent to filtering mechanism. If DisplayMemberPath is provided, path value is fetched through item property reflection and sent to filter mechanism. This works for most of the user.

However, if user needs to customize these setting/filtering mechanism, he/she can provide implementation of this interface and bind to LookUpContract property. Control will respect newly bound implementation.

**ILookUpContract.cs**

```csharp
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
```

* **`IsItemMatchingSearchString`** - This function is called to filter suggestion items in drop-down list. User entered text is passed as parameter to this function. Return true if item should be displayed in suggestion drop-down for given text. Otherwise return false.

* **`IsItemEqualToString`** - This function is used to find exact item from collection based on user entered text.

* **`CreateObject`** - This function should only be implemented if SupportsNewObjectCreation is set to true. This function is called to create new object based on provided text. For example, in [AdvanceLookUpContract](https://github.com/nilayjoshi89/BlackPearl/blob/master/BlackPearl.Controls.Demo/AdvanceLookUpContract.cs) implementation, we can create complex object by entering comma separated value in control ending with ItemSeparator (as shown in above GIF). This is just a sample implementation. You can define your own format/parsing mechanism.

* **`SupportsNewObjectCreation`** - If this property is set to false, control will not allow user to select item other than provided collection (ItemSource). If this property is set to true, control will allow creation of new object. This is useful when control should let user add new object. Also eliminates need to create separate TextBox(es) and button to add new item in existing SelectedItems/ItemSource.

* **[DefaultLookUpContract](https://github.com/nilayjoshi89/BlackPearl/blob/master/BlackPearl.Controls.Library/Control/ILookUpContract.cs)** - If no new implementation is provided to control, this DefaultLookUpContract implementation is used. This contract uses string.StartsWith for searching and string.Equals for comparison. Both comparison is invariant of culture and case.