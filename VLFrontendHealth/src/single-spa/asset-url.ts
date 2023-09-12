import { environment } from "./../environments/environment";
// In single-spa, the assets need to be loaded from a dynamic location,
// instead of hard coded to `/assets`. We use webpack public path for this.
// See https://webpack.js.org/guides/public-path/#root

export function assetUrl(url: string): string {
	if (environment.production) {
		return url;
	}
	// @ts-ignore
	const publicPath = __webpack_public_path__;
	const publicPathSuffix = publicPath.endsWith("/") ? "" : "/";
	const urlPrefix = url.startsWith("/") ? "" : "/";

	return `${publicPath}${publicPathSuffix}${urlPrefix}${url}`;
}
