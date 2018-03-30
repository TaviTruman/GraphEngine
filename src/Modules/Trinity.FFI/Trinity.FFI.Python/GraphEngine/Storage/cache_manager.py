from GraphEngine import GraphMachine


def filter_null_args(arg_list):
    return filter(lambda x: x is not None, arg_list)


class CacheManager:
    is_accessor = False
    inst = None
    module_id = -1

    def load_cell(self, cell_id):
        return self.inst.LoadCell(cell_id)

    def save_cell(self, index=None, cell_id=None, write_ahead_log_options=None):
        return self.inst.SaveCell(*filter_null_args((write_ahead_log_options, cell_id, index)))

    def get_id(self, index):
        return self.inst.CellGetId(index)

    def get_field(self, index, field_name):
        return self.inst.CellGetField(index, field_name)

    def set_field(self, index, field_name, value):
        return self.inst.CellSetField(index, field_name, value)

    def append_field(self, index, field_name, content):
        return self.inst.CellAppendField(index, field_name, content)

    def remove_field(self, index, field_name):
        return self.inst.CellRemoveField(index, field_name)

    def delete(self, index):
        return self.inst.Del(index)

    def dispose(self):
        return self.inst.Dispose()

    @staticmethod
    def remove_cell(cell_id):
        GraphMachine.storage.RemoveCellFromStorage(cell_id)
        GraphMachine.id_allocator.dealloc(cell_id)


class CellAccessorManager(CacheManager):
    def __init__(self):
        self.inst = GraphMachine.storage.CellAccessorManager()
        self.module_id = self.inst.ModuleId
        self.is_accessor = False

    def use_cell(self, cell_id, options=None, cell_type=None):
        return self.inst.UseCell(cell_id, *filter_null_args((options, cell_type)))

    def __enter__(self):
        # TODO
        return self

    def __exit__(self, exc_type, exc_val, exc_tb):
        # TODO
        self.inst.Dispose()
        del self.inst


class CellManager(CacheManager):
    def __init__(self):
        self.inst = GraphMachine.storage.CellManager()
        self.module_id = self.inst.ModuleId
        self.is_accessor = True

    def new_cell(self, cell_type, cell_id=None, content=None):
        return self.inst.NewCell(*filter_null_args((cell_type, cell_id, content)))
