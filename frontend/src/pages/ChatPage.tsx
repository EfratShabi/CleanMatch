import { useEffect, useRef, useState } from 'react'
import type { FormEvent } from 'react'
import { useMutation } from '@tanstack/react-query'
import { Link } from 'react-router-dom'
import { sendChatMessage } from '@/api/chat'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { getErrorMessage } from '@/lib/errors'
import { ChatBubble } from '@/pages/chat/ChatBubble'
import { WELCOME, newMessage, toAssistantMessage, toKnown } from '@/pages/chat/chatMessages'
import type { Message } from '@/pages/chat/chatMessages'
import type { SearchField, SearchIntent } from '@/types/api'

const MAX_LENGTH = 500 // the same limit the server enforces
const SLOW_AFTER_MS = 8000

export default function ChatPage() {
  const [messages, setMessages] = useState<Message[]>([WELCOME])
  const [text, setText] = useState('')

  // The server remembers nothing between requests, so the browser keeps the conversation:
  // what is already known, and which question was just asked.
  const [known, setKnown] = useState<SearchIntent | null>(null)
  const [asked, setAsked] = useState<SearchField | null>(null)

  const bottomRef = useRef<HTMLDivElement>(null)

  const mutation = useMutation({
    mutationFn: (message: string) => sendChatMessage({ text: message, known, asked }),
    onSuccess: (reply) => {
      setKnown(toKnown(reply.intent))
      setAsked(reply.nextField)
      setMessages((current) => [...current, toAssistantMessage(reply)])
    },
    onError: (error) => {
      setMessages((current) => [...current, newMessage('assistant', getErrorMessage(error), { isError: true })])
    },
  })

  // After a few seconds of waiting, say so, so a slow answer does not look like a freeze.
  const [slow, setSlow] = useState(false)
  useEffect(() => {
    if (!mutation.isPending) return
    const timer = setTimeout(() => setSlow(true), SLOW_AFTER_MS)
    return () => {
      clearTimeout(timer)
      setSlow(false)
    }
  }, [mutation.isPending])

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: 'smooth' })
  }, [messages, mutation.isPending, slow])

  function handleSubmit(e: FormEvent) {
    e.preventDefault()
    const message = text.trim()
    if (message === '' || mutation.isPending) return

    setMessages((current) => [...current, newMessage('user', message)])
    setText('')
    mutation.mutate(message)
  }

  return (
    <main className="mx-auto flex h-dvh max-w-xl flex-col p-4">
      <header className="flex items-center justify-between pb-3">
        <h1 className="text-lg font-medium">חיפוש בצ׳אט</h1>
        <Link to="/" className="text-sm underline">לדף הבית</Link>
      </header>

      <div className="flex-1 space-y-3 overflow-y-auto pb-3">
        {messages.map((message) => (
          <ChatBubble key={message.id} message={message} />
        ))}
        {mutation.isPending && (
          <p className="text-sm text-muted-foreground">
            {slow ? 'זה לוקח קצת יותר מהרגיל...' : 'חושבת...'}
          </p>
        )}
        <div ref={bottomRef} />
      </div>

      <form onSubmit={handleSubmit} className="flex gap-2 pt-2">
        <Input
          value={text}
          onChange={(e) => setText(e.target.value)}
          maxLength={MAX_LENGTH}
          placeholder="כתבי כאן מה את מחפשת"
          aria-label="הודעה"
          autoFocus
        />
        <Button type="submit" disabled={mutation.isPending || text.trim() === ''}>
          שליחה
        </Button>
      </form>
    </main>
  )
}
