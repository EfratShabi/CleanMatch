import { Route, Routes } from 'react-router-dom'
import { RequireAuth } from '@/auth/RequireAuth'
import ChatPage from '@/pages/ChatPage'
import CleanerProfilePage from '@/pages/CleanerProfilePage'
import HomePage from '@/pages/HomePage'
import SearchPage from '@/pages/SearchPage'
import LoginPage from '@/pages/LoginPage'
import SignupPage from '@/pages/SignupPage'

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<HomePage />} />
      <Route
        path="/search"
        element={
          <RequireAuth roles={['customer']}>
            <SearchPage />
          </RequireAuth>
        }
      />
      <Route
        path="/chat"
        element={
          <RequireAuth roles={['customer']}>
            <ChatPage />
          </RequireAuth>
        }
      />
      <Route
        path="/cleaners/:id"
        element={
          <RequireAuth>
            <CleanerProfilePage />
          </RequireAuth>
        }
      />
      <Route path="/login" element={<LoginPage />} />
      <Route path="/signup" element={<SignupPage />} />
    </Routes>
  )
}
