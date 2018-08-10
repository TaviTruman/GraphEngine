from Redy.ADT.Core import data
from typing import Callable
from .mangling import mangling_code


@data
class Verb:
    # TODO: to report jetbrains f-string error issues.
    LGet: lambda list_name: f'{list_name}{mangling_code}Get'
    LSet: lambda list_name: f'{list_name}{mangling_code}Set'
    LCount: lambda list_name: f'Count{mangling_code}{list_name}'
    LContains: lambda list_name: f'Contains{mangling_code}{list_name}'
    LInsertAt: lambda list_name: f'Insert{mangling_code}{list_name}'
    LRemoveAt: lambda list_name: f'Remove{mangling_code}{list_name}'
    LAppend: lambda list_name: f'Append{mangling_code}{list_name}'

    BGet: lambda typename: f"Get{mangling_code}{typename}"
    BSet: lambda typename: f"Set{mangling_code}{typename}"
    BNew: lambda typename: f"New{mangling_code}{typename}"

    SGet: lambda typename, member_name: f"{typename}{mangling_code}Get{mangling_code}{member_name}"
    SSet: lambda typename, member_name: f"{typename}{mangling_code}Set{mangling_code}{member_name}"

    def __str__(self):
        return self.__inst_str__


LSet: Callable[[str], Verb] = Verb.LSet
LGet: Callable[[str], Verb] = Verb.LGet
LCount: Callable[[str], Verb] = Verb.LCount
LContains: Callable[[str], Verb] = Verb.LContains
LInsertAt: Callable[[str], Verb] = Verb.LInsertAt
LRemoveAt: Callable[[str], Verb] = Verb.LRemoveAt
LAppend: Callable[[str], Verb] = Verb.LAppend

BGet: Callable[[str], Verb] = Verb.BGet
BSet: Callable[[str], Verb] = Verb.BSet
BNew: Callable[[str], Verb] = Verb.BNew

SGet: Callable[[str, str], Verb] = Verb.SGet
SSet: Callable[[str, str], Verb] = Verb.SSet
