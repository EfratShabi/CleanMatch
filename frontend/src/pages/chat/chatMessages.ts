import type { ChatReply, CleanerSearchResult, SearchField, SearchIntent, SearchParams } from '@/types/api'

export type Message = {
  id: string
  role: 'user' | 'assistant'
  text: string
  isError?: boolean
  // present on an answer that carries search results
  results?: CleanerSearchResult[]
  search?: SearchParams
}

const QUESTIONS: Record<SearchField, string> = {
  Date: 'לאיזה יום?',
  Start: 'באיזו שעה?',
  Hours: 'לכמה שעות?',
}

export const newMessage = (role: Message['role'], text: string, extra: Partial<Message> = {}): Message => ({
  id: crypto.randomUUID(),
  role,
  text,
  ...extra,
})

export const WELCOME = newMessage(
  'assistant',
  'היי! ספרי לי מה את מחפשת, למשל: "מנקה מחר בבוקר ל-3 שעות עד 80 ש״ח".',
)

// The server also sends nextMissingField inside the intent; only the four real fields go back as "known".
export function toKnown(intent: SearchIntent): SearchIntent {
  return { date: intent.date, start: intent.start, hours: intent.hours, maxPrice: intent.maxPrice }
}

export function toAssistantMessage(reply: ChatReply): Message {
  switch (reply.status) {
    case 'NotUnderstood':
      return newMessage(
        'assistant',
        reply.nextField
          ? `לא הבנתי, אפשר לנסח אחרת? ${QUESTIONS[reply.nextField]}`
          : 'לא הבנתי, אפשר לנסח אחרת?',
      )
    case 'NeedsMore':
      return newMessage('assistant', QUESTIONS[reply.nextField!])
    case 'Results': {
      const { intent } = reply
      const results = reply.results ?? []
      if (results.length === 0) {
        return newMessage('assistant', 'לא נמצאו מנקות מתאימות. אפשר לנסות יום, שעה או מחיר אחרים.')
      }
      return newMessage('assistant', `מצאתי ${results.length} מנקות:`, {
        results,
        search: {
          date: intent.date!,
          start: intent.start!.slice(0, 5),
          hours: intent.hours!,
          maxPrice: intent.maxPrice,
        },
      })
    }
  }
}
