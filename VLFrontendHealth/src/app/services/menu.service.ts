import { Injectable } from "@angular/core";

import { AuthService } from "./../services/auth.service";

@Injectable({
	providedIn: "root"
})
export class MenuService {
	public collapsed = false;
	public showSideNav = true;
	public isExpanded = false;
	private window$: any = window;
	constructor(private _authService: AuthService) {}

	public collapse(open: boolean = null): void {
		if (open !== null) {
			this.collapsed = open;
		} else {
			this.collapsed = !this.collapsed;
		}
	}

	public hideMenu(): void {
		const event = new CustomEvent(this.window$.eventsOFApp.SHOW_SIDE_NAV, {
			cancelable: true,
			detail: {
				showSideNav: false
			}
		});
		dispatchEvent(event);
	}

	public showMenu(): void {
		const event = new CustomEvent(this.window$.eventsOFApp.SHOW_SIDE_NAV, {
			cancelable: true,
			detail: {
				showSideNav: true
			}
		});
		dispatchEvent(event);
	}

	public validResource(idResource: number): any {
		let resource = [];
		this._authService.accessResource$.subscribe((value) => (resource = value));
		return resource.find((u) => u.resource.id === idResource);
	}
}
