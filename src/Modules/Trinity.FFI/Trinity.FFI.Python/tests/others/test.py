## -*- coding: utf-8 -*-
# """
# Created on Mon Jan 29 16:18:13 2018
#
# @author: v-wazhao
# """
#
#
from __future__ import print_function
import GraphEngine.ffi.GraphEngine as ge

global c
Ignore = object()
Succeed = []
Failed = []


def test(title, args, out=None, expected=Ignore):
    print(title)
    head, *tail = args
    try:
        this = head(*tail)
        print(this)
        if out:
            globals()[out] = this
        if expected is not Ignore:
            assert expected == this
        print('finish {}'.format(title))
        Succeed.append(title)
    except Exception as e:
        print('{} failed, error: {}'.format(title, e))
        Failed.append(title)
    print('===================')


print("start-test")

test('[new cell]',
     [ge.NewCell_1, "C1"], out='c')

print(c.m_cell)

test('[has optional field]',
     [c.HasField, 'foo'], expected=1)

test('[remove optional field]',
     [c.RemoveField, 'baz'])

test('[has field for removed optional attr]',
     [c.HasField, 'baz'])

test('[get trivial field]',
     [c.GetField, 'foo'], out='d')

test('[delete cell]',
     [lambda: exec('del c', globals())])

print("end-test")

print('succeed:\n\t', '\n\t'.join(Succeed), sep='')
print('failed:\n\t', '\n\t'.join(Failed), sep='')
