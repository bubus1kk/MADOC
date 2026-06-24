using MADOC.Domain.Printing.Anchors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MADOC.Tests.Domain.Printing.Anchors;

[TestClass]
public class PrintAnchorProfileTests
{
    [TestMethod]
    public void Constructor_Should_Create_Default_Prefix_From_Document_Type()
    {
        var profile = new PrintAnchorProfile<ProfileDocument>();

        Assert.AreEqual("profile", profile.Prefix);
    }

    [TestMethod]
    public void Constructor_Should_Use_Custom_Prefix()
    {
        var profile = new PrintAnchorProfile<ProfileDocument>("custom_profile");

        Assert.AreEqual("custom_profile", profile.Prefix);
        Assert.IsTrue(profile.Contains("custom_profile.public_text"));
    }

    [TestMethod]
    public void Constructor_Should_Throw_When_Custom_Prefix_Is_Empty()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
        {
            _ = new PrintAnchorProfile<ProfileDocument>(string.Empty);
        });
    }

    [TestMethod]
    public void Anchors_Should_Register_All_Public_Readable_Properties()
    {
        var profile = new PrintAnchorProfile<ProfileDocument>();

        Assert.IsTrue(profile.Contains("profile.public_text"));
        Assert.IsTrue(profile.Contains("profile.number"));
        Assert.IsTrue(profile.Contains("profile.computed_text"));
        Assert.IsTrue(profile.Contains("profile.inherited_text"));
    }

    [TestMethod]
    public void Anchors_Should_Register_Computed_Properties()
    {
        var profile = new PrintAnchorProfile<ProfileDocument>();

        var document = new ProfileDocument
        {
            PublicText = "Основное значение",
            Number = 5
        };

        var definition = profile.GetRequired("profile.computed_text");
        var value = definition.GetValue(document);

        Assert.AreEqual("Основное значение / 5", value);
    }

    [TestMethod]
    public void Anchors_Should_Register_Inherited_Public_Properties()
    {
        var profile = new PrintAnchorProfile<ProfileDocument>();

        var definition = profile.GetRequired("profile.inherited_text");

        Assert.AreEqual(nameof(BaseProfileDocument.InheritedText), definition.FieldName);
    }

    [TestMethod]
    public void Anchors_Should_Not_Register_Indexers()
    {
        var profile = new PrintAnchorProfile<ProfileDocument>();

        Assert.IsFalse(profile.Contains("profile.item"));
    }

    [TestMethod]
    public void Anchors_Should_Not_Register_Properties_With_Private_Getter()
    {
        var profile = new PrintAnchorProfile<ProfileDocument>();

        Assert.IsFalse(profile.Contains("profile.private_getter_text"));
    }

    [TestMethod]
    public void Anchors_Should_Not_Register_WriteOnly_Properties()
    {
        var profile = new PrintAnchorProfile<ProfileDocument>();

        Assert.IsFalse(profile.Contains("profile.write_only_text"));
    }

    [TestMethod]
    public void Anchors_Should_Return_All_Registered_Anchors()
    {
        var profile = new PrintAnchorProfile<ProfileDocument>();

        var keys = profile.Anchors
            .Select(anchor => anchor.Key.Value)
            .ToList();

        CollectionAssert.Contains(keys, "profile.public_text");
        CollectionAssert.Contains(keys, "profile.number");
        CollectionAssert.Contains(keys, "profile.computed_text");
        CollectionAssert.Contains(keys, "profile.inherited_text");
    }

    [TestMethod]
    public void TryGet_Should_Return_True_When_Anchor_Exists()
    {
        var profile = new PrintAnchorProfile<ProfileDocument>();

        var result = profile.TryGet(
            new AnchorKey("profile.public_text"),
            out var definition);

        Assert.IsTrue(result);
        Assert.IsNotNull(definition);
        Assert.AreEqual(nameof(ProfileDocument.PublicText), definition.FieldName);
    }

    [TestMethod]
    public void TryGet_Should_Return_False_When_Anchor_Does_Not_Exist()
    {
        var profile = new PrintAnchorProfile<ProfileDocument>();

        var result = profile.TryGet(
            new AnchorKey("profile.unknown"),
            out var definition);

        Assert.IsFalse(result);
        Assert.IsNull(definition);
    }

    [TestMethod]
    public void GetRequired_Should_Return_Definition_When_Anchor_Exists()
    {
        var profile = new PrintAnchorProfile<ProfileDocument>();

        var definition = profile.GetRequired("profile.public_text");

        Assert.AreEqual(nameof(ProfileDocument.PublicText), definition.FieldName);
    }

    [TestMethod]
    public void GetRequired_Should_Throw_When_Anchor_Does_Not_Exist()
    {
        var profile = new PrintAnchorProfile<ProfileDocument>();

        Assert.ThrowsExactly<InvalidOperationException>(() =>
        {
            profile.GetRequired("profile.unknown");
        });
    }

    [TestMethod]
    public void Anchors_Should_Throw_When_Document_Has_No_Public_Readable_Properties()
    {
        var profile = new PrintAnchorProfile<EmptyDocument>();

        Assert.ThrowsExactly<InvalidOperationException>(() =>
        {
            _ = profile.Anchors;
        });
    }

    [TestMethod]
    public void Custom_Profile_Should_Allow_Filtering_Properties()
    {
        var profile = new OnlyPublicTextProfile();

        Assert.IsTrue(profile.Contains("profile.public_text"));
        Assert.IsFalse(profile.Contains("profile.number"));
        Assert.IsFalse(profile.Contains("profile.computed_text"));
    }

    private class BaseProfileDocument
    {
        public string InheritedText { get; set; } = string.Empty;
    }

    private sealed class ProfileDocument : BaseProfileDocument
    {
        public string PublicText { get; set; } = string.Empty;

        public int Number { get; set; }

        public string ComputedText
        {
            get
            {
                return $"{PublicText} / {Number}";
            }
        }

        public string PrivateGetterText { private get; set; } = string.Empty;

        public string WriteOnlyText
        {
            set
            {
                _ = value;
            }
        }

        public string this[int index]
        {
            get
            {
                return index.ToString();
            }
        }
    }

    private sealed class EmptyDocument
    {
        public string WriteOnlyText
        {
            set
            {
                _ = value;
            }
        }
    }

    private sealed class OnlyPublicTextProfile : PrintAnchorProfile<ProfileDocument>
    {
        protected override IReadOnlyList<System.Reflection.PropertyInfo> GetPropertiesForAnchors()
        {
            return new[]
            {
                typeof(ProfileDocument).GetProperty(nameof(ProfileDocument.PublicText))!
            };
        }
    }
}
