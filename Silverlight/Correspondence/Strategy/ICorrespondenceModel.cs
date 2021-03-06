﻿using System;
using System.Collections.Generic;

namespace Correspondence.Strategy
{
	public interface ICorrespondenceModel
	{
		void RegisterAllFactTypes(Community community, IDictionary<Type, IFieldSerializer> fieldSerializerByType);
	}
}
