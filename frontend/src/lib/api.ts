import { clearToken, getToken } from '@/lib/token'

const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5184'

export class ApiError extends Error {
  status: number

  constructor(status: number, message: string) {
    super(message)
    this.name = 'ApiError'
    this.status = status
  }
}

type ApiOptions = {
  method?: 'GET' | 'POST' | 'PUT' | 'DELETE'
  body?: unknown
  // false for requests that must work without a token (login, signup) and must not trigger the redirect on 401
  auth?: boolean
  signal?: AbortSignal
}

// The server sends errors as ProblemDetails: { title, detail, status }.
async function readErrorMessage(response: Response): Promise<string> {
  try {
    const problem = await response.json()
    return problem.detail ?? problem.title ?? 'משהו השתבש. נסי שוב.'
  } catch {
    return 'משהו השתבש. נסי שוב.'
  }
}

export async function apiFetch<T>(path: string, options: ApiOptions = {}): Promise<T> {
  const { method = 'GET', body, auth = true, signal } = options

  const headers: Record<string, string> = {}
  if (body !== undefined) {
    headers['Content-Type'] = 'application/json'
  }
  const token = getToken()
  if (auth && token) {
    headers.Authorization = `Bearer ${token}`
  }

  let response: Response
  try {
    response = await fetch(`${BASE_URL}${path}`, {
      method,
      headers,
      body: body === undefined ? undefined : JSON.stringify(body),
      signal,
    })
  } catch (error) {
    if (error instanceof DOMException && error.name === 'AbortError') throw error
    throw new ApiError(0, 'אין חיבור לשרת. בדקי את החיבור ונסי שוב.')
  }

  if (response.status === 401 && auth) {
    // The token is missing, expired or invalid: forget it and send the user to log in.
    clearToken()
    if (window.location.pathname !== '/login') {
      window.location.assign('/login')
    }
    throw new ApiError(401, 'צריך להתחבר מחדש.')
  }

  if (!response.ok) {
    throw new ApiError(response.status, await readErrorMessage(response))
  }

  // Some endpoints answer with an empty body (e.g. 200 with no content).
  const text = await response.text()
  return (text ? JSON.parse(text) : undefined) as T
}
