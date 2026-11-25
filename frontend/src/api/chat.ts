import { apiFetch } from '@/lib/api'
import type { ChatReply, ChatRequest } from '@/types/api'

export function sendChatMessage(request: ChatRequest) {
  return apiFetch<ChatReply>('/api/cleaners/search/chat', { method: 'POST', body: request })
}
