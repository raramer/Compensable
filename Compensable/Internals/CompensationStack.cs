using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Compensable
{
    internal sealed class CompensationStack<TCompensation> where TCompensation : Delegate
    {
        private readonly ConcurrentStack<(ConcurrentStack<TCompensation> Compensations, Tag Tag)> _taggedCompensations;

        internal CompensationStack()
        {
            _taggedCompensations = new ConcurrentStack<(ConcurrentStack<TCompensation> Compensations, Tag Tag)>();
        }

        internal void AddCompensation(TCompensation compensation, Tag compensateAtTag)
        {
            Validate.Compensation(compensation);

            if (compensateAtTag == null)
            {
                // create compensation stack
                var compensations = new ConcurrentStack<TCompensation>();

                // add compensation
                compensations.Push(compensation);

                // add stack to tagged compensations
                _taggedCompensations.Push((compensations, null));
            }
            else
            {
                // add compensation to tagged compensation
                _taggedCompensations.First(tc => tc.Tag == compensateAtTag).Compensations.Push(compensation);
            }
        }

        internal Tag AddTag(string label)
        {
            var tag = new Tag(label);
            _taggedCompensations.Push((new ConcurrentStack<TCompensation>(), tag));
            return tag;
        }

        internal void Clear()
        {
            _taggedCompensations.Clear();
        }

        internal bool TryPeek(out TCompensation compensation)
        {
            compensation = default(TCompensation);

            (ConcurrentStack<TCompensation> Compensations, Tag tag) taggedCompensations;
            if (!_taggedCompensations.TryPeek(out taggedCompensations))
                return false;

            if (!taggedCompensations.Compensations.TryPeek(out compensation))
            {
                _taggedCompensations.TryPop(out _);
                return TryPeek(out compensation);
            }

            return true;
        }

        internal bool TryPop(out TCompensation compensation)
        {
            compensation = default(TCompensation);

            (ConcurrentStack<TCompensation> Compensations, Tag tag) taggedCompensations;
            if (!_taggedCompensations.TryPeek(out taggedCompensations))
                return false;

            if (!taggedCompensations.Compensations.TryPop(out compensation))
            {
                _taggedCompensations.TryPop(out _);
                return false;
            }

            if (!taggedCompensations.Compensations.Any())
                _taggedCompensations.TryPop(out _);

            return true;
        }

        internal void ValidateTag(Tag tag)
        {
            if (tag != null && !_taggedCompensations.Any(c => c.Tag == tag))
                throw new TagNotFoundException();
        }
    }
}