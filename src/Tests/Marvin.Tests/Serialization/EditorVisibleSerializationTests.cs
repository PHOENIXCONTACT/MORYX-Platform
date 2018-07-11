﻿using System.Linq;
using Marvin.Serialization;
using NUnit.Framework;

namespace Marvin.Tests
{
    [TestFixture]
    public class EditorVisibleSerializationTests
    {
        [Test(Description = "Retrieve only all visible only")]
        public void GetMethodsWhereEditorVisibleAttributeIsSet()
        {
            // Arrange
            
            // Act
            var methods = EntryConvert.EncodeMethods(new EditorVisibleMixed(), new EditorVisibleSerialization()).ToList();
            
            // Assert
            Assert.AreEqual(1, methods.Count);
            Assert.AreEqual(nameof(EditorVisibleMixed.Method1), methods.First().Name);
        }

        [Test(Description = "Retrieve only all visible properties")]
        public void GetPropertiesWhereEditorVisibleAttributeIsSet()
        {
            // Arrange

            // Act
            var entries = EntryConvert.EncodeObject(new EditorVisibleMixed(), new EditorVisibleSerialization()).ToList();

            // Assert
            Assert.IsNotNull(entries);
            Assert.AreEqual(2, entries.Count);
            Assert.AreEqual(nameof(EditorVisibleMixed.Property1), entries[0].Key.Name);
            Assert.AreEqual(nameof(EditorVisibleMixed.Property2), entries[1].Key.Name);
        }

        [Test(Description = "Retrieve only all visible methods and properties on a class where no EditorVisible attributes are set")]
        public void GetNoMethodsAndPropertiesWhereNoEditorVisibleAttributeIsSet()
        {
            // Arrange

            // Act
            var entries = EntryConvert.EncodeObject(new NoEditorVisibleSet(), new EditorVisibleSerialization());

            // Assert
            Assert.AreEqual(0, entries.Count());
        }
    }
}
