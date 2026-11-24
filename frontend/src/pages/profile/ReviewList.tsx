import type { Feedback } from '@/types/api'

const stars = (rating: number) => '★'.repeat(rating) + '☆'.repeat(5 - rating)

export function ReviewList({ reviews }: { reviews: Feedback[] }) {
  if (reviews.length === 0) {
    return <p className="text-sm text-muted-foreground">אין חוות דעת עדיין.</p>
  }

  return (
    <ul className="space-y-3">
      {reviews.map((review) => (
        <li key={review.id} className="text-sm">
          <p>
            <span aria-label={`${review.rating} מתוך 5`}>{stars(review.rating)}</span>{' '}
            <span className="text-muted-foreground">
              {new Date(review.createdAt).toLocaleDateString('he-IL')}
            </span>
          </p>
          {review.textOfFeedback && <p>{review.textOfFeedback}</p>}
        </li>
      ))}
    </ul>
  )
}
