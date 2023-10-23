shader_type spatial;
render_mode cull_front, depth_prepass_alpha, depth_draw_opaque, unshaded;

uniform sampler2D DEPTH_TEXTURE : hint_depth_texture, filter_linear_mipmap;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;
uniform vec4 outlineColor : source_color = vec4(0.0, 0.0, 0.0, 1.0);
const float distanceFalloff = 1.0;
const float smoothingCutoff = 0.1;
const float smoothingMax = 0.15;

float absDiff (float a, float b)
{
	return abs(abs(a)-abs(b));
}

float linearDepth (in sampler2D depth, in vec2 screenUV, in mat4 invProjectionMatrix)
{
	float rawDepth = texture(depth, screenUV)[0];
	vec3 ndc = vec3(screenUV * 2.0 - 1.0, rawDepth);
	vec4 viewSpace = invProjectionMatrix * vec4(ndc, 1.0);
	viewSpace.xyz /= viewSpace.w;

	return -viewSpace.z;
}

void fragment ()
{
	ALBEDO = outlineColor.rgb;

	float depth = linearDepth(DEPTH_TEXTURE, SCREEN_UV, INV_PROJECTION_MATRIX);

	vec2 screenSize = vec2(textureSize(SCREEN_TEXTURE, 1));

	vec2 pixelSize = (distanceFalloff / depth) / vec2(screenSize.x, screenSize.y);

	float dU = linearDepth(DEPTH_TEXTURE, SCREEN_UV + vec2(0.0, pixelSize.y), INV_PROJECTION_MATRIX);
	float dD = linearDepth(DEPTH_TEXTURE, SCREEN_UV + vec2(0.0, -pixelSize.y), INV_PROJECTION_MATRIX);
	float dR = linearDepth(DEPTH_TEXTURE, SCREEN_UV + vec2(pixelSize.x, 0.0), INV_PROJECTION_MATRIX);
	float dL = linearDepth(DEPTH_TEXTURE, SCREEN_UV + vec2(-pixelSize.x, 0.0), INV_PROJECTION_MATRIX);

	ALPHA = absDiff(depth, dU) + absDiff(depth, dD) + absDiff(depth, dR) + absDiff(depth, dL);

	ALPHA = smoothstep(smoothingCutoff, smoothingMax, ALPHA);
	ALPHA *= outlineColor.a;
	ALPHA = clamp(ALPHA, 0.0, 1.0);
}