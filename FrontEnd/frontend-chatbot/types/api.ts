// types/api.ts
export interface ApiResponse<T = any> {
  success: boolean
  data?: T
  message?: string
  errors?: string[]
}

export interface ContactInfo {
  name: string
  email: string
  phone: string
  linkedin?: string
  github?: string
  location?: string
}

export interface F1Info {
  favoriteTeam?: string
  favoriteDriver?: string
  favoriteSeason?: string
  currentChampion?: string
}
