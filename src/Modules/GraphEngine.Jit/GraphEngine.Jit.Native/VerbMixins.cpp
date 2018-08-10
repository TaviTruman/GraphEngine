#include "VerbMixins.h"

namespace Mixin
{
#define MIXIN(x) {VerbCode::VC_##x, new x()}
    static std::unordered_map<VerbCode, VerbMixin*> s_mixins =
    {
        MIXIN(BNew),
        MIXIN(BGet),
        MIXIN(BSet),
        MIXIN(BCmp),
        MIXIN(BLe),
        MIXIN(BLt),
        MIXIN(BGe),
        MIXIN(BGt),
        MIXIN(BRefEq),

        MIXIN(LGet),
        MIXIN(LSet),
        MIXIN(LContains),
        MIXIN(LCount),
        MIXIN(LInsertAt),
        MIXIN(LAppend),
        MIXIN(LRemoveAt),

        MIXIN(SGet),
        MIXIN(SSet),
    };

    VerbMixin* Make(VerbCode code)
    {
        auto i = s_mixins.find(code);
        if (i == s_mixins.cend())
        {
            print("Mixin::Make: unexpected verb code");
            debug(code);
            throw;
        }
        return i->second;
    }

    static VerbMixin* Make(Verb* v)
    {
        return Make(v->Code);
    }

    TypeId::Id GetReturnId(FunctionDescriptor* fdesc)
    {
        TypeId::Id ret = TypeId::Id::kVoid;
        VerbSequence seq(fdesc);
        while (seq.Next()) { if (Make(seq.pcurrent)->GetRetId(seq, ret)) break; }
        return ret;
    }

    void GetArgument(IN FunctionDescriptor* fdesc, OUT uint8_t* &pargs, OUT int32_t& nargs)
    {
        /* 1st argument is always CellAccessor* */
        std::vector<uint8_t> vec{ TypeId::kUIntPtr };
        VerbSequence seq(fdesc);
        while (seq.Next()) { if (Make(seq.pcurrent)->GetArgs(seq, vec)) break; }

        nargs = vec.size();
        pargs = (uint8_t*)malloc(nargs * sizeof(uint8_t));
        std::copy(vec.begin(), vec.end(), pargs);
    }

    bool WouldResize(FunctionDescriptor* fdesc)
    {
        VerbSequence seq(fdesc);
        while (seq.Next()) { if (Make(seq.pcurrent)->WouldResize(seq)) return true; }
        return false;
    }

    void DoDispatch(X86Compiler &cc, FuncCtx& ctx, VerbSequence& seq)
    {
        while (seq.Next() && !ctx.returned) { Make(seq.pcurrent)->Dispatch(cc, ctx, seq); }
    }

    bool SequenceNext(VerbSequence& seq)
    {
        if (++seq.pcurrent == seq.pend) return false;
        debug(seq.pcurrent->Code);
        debug(seq.ptype->get_QualifiedName());
        Make(seq.pcurrent)->SequenceNext(seq);
        return true;
    }
}
