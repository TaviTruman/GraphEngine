%module ffi

%{
#define SWIG_FILE_WITH_INIT
#include "Trinity.FFI.SWIG.h"
%}

%include "attribute.i"
%include "std_vector.i"

namespace std {
	%template(tdesc_vec) vector<TypeDescriptor>;
	%template(ptdesc_vec) vector<TypeDescriptor*>;
	%template(pmdesc_vec) vector<MemberDescriptor*>;
	%template(padesc_vec) vector<AttributeDescriptor*>;
};

%newobject NewCell;
%newobject LoadCell;

%attribute(TypeDescriptor, char*, TypeName, get_TypeName)
%attribute(TypeDescriptor, char*, QualifiedName, get_QualifiedName)
%attribute(TypeDescriptor, int, TypeCode, get_TypeCode)
%attribute(TypeDescriptor, std::vector<TypeDescriptor*>, ElementType, get_ElementType)
%attribute(TypeDescriptor, std::vector<MemberDescriptor*>, Members, get_Members)
%attribute(TypeDescriptor, std::vector<AttributeDescriptor*>, TSLAttributes, get_TSLAttributes)

%include "Trinity.FFI.Schema.h"
%include "Trinity.FFI.SWIG.h"

