import { UserHobbyResponse } from './user.model';
import { ProjectResponse } from './project.model';

export interface ChatMessageResponse {
  id: number;
  content: string;
  userId: number;
  userName: string;
  userProfileImageUrl?: string;
  hobbyId: number;
  hobbyName: string;
  createdAt: string;
}

export interface PublicUserProfileResponse {
  id: number;
  firstName: string;
  lastName: string;
  username: string;
  profileImageUrl?: string;
  createdAt: string;
  hobbies: UserHobbyResponse[];
  projects: ProjectResponse[];
}

export interface CommunityUserResponse {
  id: number;
  firstName: string;
  lastName: string;
  username: string;
  profileImageUrl?: string;
  hobbyNames: string[];
  projectCount: number;
}
