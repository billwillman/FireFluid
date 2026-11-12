using System;
using System.Collections.Generic;

[Serializable]
public class StructuredAttributePaths
{
	public string attribute;

	public string fullAttributePath;

	public List<StructuredAttributePaths> subAttributes;

	public StructuredAttributePaths(string attribute)
	{
		this.attribute = attribute;
		subAttributes = new List<StructuredAttributePaths>();
	}
}
