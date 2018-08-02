from .type_sys import *
from .type_map import *
from .type_verbs import *
from .mangling import mangling

from Redy.Tools.TypeInterface import Module
from Redy.Magic.Pattern import Pattern
from Redy.Magic.Classic import cast, execute
from typing import Type, List, Tuple


def type_spec_to_name(typ: TSLTypeSpec) -> str:
    """
    consistent to Trinity.FFI.Metagen: MetaGen.`make'name`.
    """
    if isinstance(typ, ListSpec):
        return 'List{_}{elem}'.format(_=mangling_code, elem=type_spec_to_name(typ.elem_type))
    if isinstance(typ, CellSpec):
        return 'Cell{_}{name}'.format(_=mangling_code, name=typ.name)
    elif isinstance(typ, StructSpec):
        return 'Struct{_}{name}'.format(_=mangling_code, name=typ.name)
    else:
        return str(typ)


def tsl_build_src_code(code: str) -> Module:
    raise NotImplemented


def typename_manging(typ: TSLObject) -> str:
    raise NotImplemented


@Pattern
def tsl_generate_methods(tsl_session, cls_def: Type[TSLObject]):
    if issubclass(cls_def, TSLStruct):
        return TSLStruct
    elif issubclass(cls_def, TSLList):
        return TSLList
    else:
        print(cls_def.__name__, cls_def.__bases__)
        raise TypeError


def make_setter_getter_for_primitive(_getter, _setter):
    @property
    def getter(self: TSLStruct):
        return _getter(self.__accessor__)

    @getter.setter
    def setter(self, value):
        assert not isinstance(value, TSLObject)
        _setter(self.__accessor__, value)

    return setter


def make_setter_getter_for_general(_getter, _setter, object_cls: Type[TSLObject]):
    @property
    def getter(self: TSLStruct):
        new = object_cls.__new__(object_cls)
        new.__accessor__ = _getter(self.__accessor__)
        return new

    @getter.setter
    def setter(self, value):
        assert isinstance(value, TSLObject)
        _setter(self.__accessor__, value.__accessor__)

    return setter


@tsl_generate_methods.match(TSLStruct)
def tsl_generate_methods(tsl_session, cls_def: Type[TSLObject]):
    spec: StructSpec = cls_def.get_spec()
    typename = type_spec_to_name(spec)
    for field_name, field_spec in spec.fields.items():

        _field_name = mangling(field_name)

        getter_name = SGet(typename, _field_name).__str__()
        setter_name = SSet(typename, _field_name).__str__()
        _getter = getattr(tsl_session.module, getter_name)
        _setter = getattr(tsl_session.module, setter_name)

        # SGet/ SSet
        # cell.lst = tsl_lst
        # cell.foo = 1  # primitive
        # cell.some_struct = some_struct
        # print (cell.bar)

        if isinstance(field_spec, PrimitiveSpec):
            setter = make_setter_getter_for_primitive(_getter, _setter)
        else:
            field_cls = tsl_session.type_specs_to_type[field_spec]
            setter = make_setter_getter_for_general(_getter, _setter, field_cls)

        setattr(cls_def, field_name, setter)

    # BGet. deepcopy.
    # new_cell = cell.deepcopy()

    deepcopy_fn_name = BGet(typename).__str__()
    _deepcopy = getattr(tsl_session.module, deepcopy_fn_name)

    def deepcopy(self) -> cls_def:
        new = cls_def.__new__(cls_def)
        new.__accessor__ = _deepcopy(self.__accessor__)
        return new

    cls_def.deepcopy = deepcopy

    # BSet. change value by reference.
    # cell &= another_cell
    reference_assign_fn_name = BGet(typename).__str__()
    _reference_assign = getattr(tsl_session.module, reference_assign_fn_name)

    def reference_assign(self, value):
        assert isinstance(value, TSLObject)
        _reference_assign(self.__accessor__, value.__accessor__)

    cls_def.__iand__ = reference_assign

    # New a Cell/Struct
    # my_cell = MyCell()
    new_struct_fn_name = BNew(typename).__str__()
    _new_struct = getattr(tsl_session.module, new_struct_fn_name)

    def new_struct(self):
        self.__accessor__ = _new_struct()

    cls_def.__init__ = new_struct


@tsl_generate_methods.match(TSLList)
def tsl_generate_methods(tsl_session, cls_def):
    spec: ListSpec = cls_def.get_spec()

    typename = type_spec_to_name(spec)
    (_get, _set, _count, _contains, _insert, _remove, _append, _deepcopy, _reference_assign, _new_lst) = map(
        lambda verb: getattr(tsl_session.module, str(verb)),
        [
            LGet(typename),
            LSet(typename),
            LCount(typename),
            LContains(typename),
            LInsertAt(typename),
            LRemoveAt(typename),
            LAppend(typename),
            BGet(typename),
            BSet(typename),
            BNew(typename)
        ])

    # Index getter/setter, insert, remove, append
    if isinstance(spec.elem_type, PrimitiveSpec):
        def __getitem__(self, i: int):
            return _get(self.__accessor__, i)

        def __setitem__(self, i: int, value):
            assert not isinstance(value, (TSLList, TSLStruct))
            _set(self.__accessor__, i, value)

        def insert(self, i: int, value) -> bool:
            return _insert(self.__accessor__, i, value)

        def append(self, value):
            _append(self.__accessor__, value)

    else:
        field_cls = tsl_session.type_specs_to_type[spec.elem_type]

        def __getitem__(self, i: int):
            new = field_cls.__new__(field_cls)
            new.__accessor__ = _get(self.__accessor__, i)
            return new

        def __setitem__(self, i: int, value):
            assert isinstance(value, (TSLList, TSLStruct))
            _set(self.__accessor__, i, value.__accessor__)

        def insert(self, i: int, value):
            return _insert(self.__accessor__, i, value.__accessor__)

        def append(self, value):
            _append(self.__accessor__, value.__accessor__)

    def remove_at(self, i: int):
        return _remove(self.__accessor__, i)

    cls_def.__getitem__ = __getitem__
    cls_def.__setitem__ = __setitem__
    cls_def.append = append
    cls_def.remove_at = remove_at
    cls_def.insert = insert

    # len(lst: TSLList) -> int
    def __len__(self):
        return _count(self.__accessor__)

    cls_def.__len__ = __len__

    # elem in lst
    if isinstance(spec.elem_type, PrimitiveSpec):
        def __contains__(self, elem):
            assert not isinstance(elem, (TSLStruct, TSLList))
            return _contains(self.__accessor__, elem)
    else:
        def __contains__(self, elem):
            assert isinstance(elem, (TSLList, TSLStruct))
            return _contains(self.__accessor__, elem.__accessor__)

    cls_def.__contains__ = __contains__

    # New a List
    def new_lst(self):
        self.__accessor__ = _new_lst()

    cls_def.__init__ = new_lst
