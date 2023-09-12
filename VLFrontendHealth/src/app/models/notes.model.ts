import { NoteType } from "./note-type.model";

export class Note {
	id: number;
	idPerson: number;
	type: NoteType;
	creation: string;
	expiration: string;
	motive: string;
	description: string;
	revision: boolean;
	color: string;
}
