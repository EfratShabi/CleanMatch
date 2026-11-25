import { CleanerCard } from '@/pages/search/CleanerCard'
import type { Message } from '@/pages/chat/chatMessages'

export function ChatBubble({ message }: { message: Message }) {
  const mine = message.role === 'user'

  return (
    <div className={mine ? 'flex justify-start' : 'flex justify-end'}>
      <div
        className={[
          'max-w-[85%] space-y-3 rounded-2xl px-4 py-2 text-sm',
          mine ? 'bg-primary text-primary-foreground' : 'bg-muted',
          message.isError ? 'text-destructive' : '',
        ].join(' ')}
      >
        <p>{message.text}</p>
        {message.results && message.search && (
          <div className="space-y-3">
            {message.results.map((cleaner) => (
              <CleanerCard key={cleaner.id} cleaner={cleaner} search={message.search!} />
            ))}
          </div>
        )}
      </div>
    </div>
  )
}
